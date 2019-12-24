using System;
using System.IO;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    public Task Verify(string input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        var extension = settings.ExtensionOrTxt();

        async Task LaunchDiff(FilePair pair)
        {
            await FileHelpers.WriteText(pair.Received, input);
            if (DiffTools.TryGetTextDiff(extension, out var diffTool))
            {
                FileHelpers.WriteEmptyText(pair.Verified);
                DiffRunner.Launch(diffTool, pair.Received, pair.Verified);
            }
        }

        var file = GetFileNames(extension, settings.Namer);

        var scrubbedInput = ScrubInput(input, settings);
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (File.Exists(file.Verified))
        {
            return VerifyExisting(scrubbedInput, file, LaunchDiff);
        }

        return VerifyFirstTime(file, LaunchDiff);
    }

    static string ScrubInput(string input, VerifySettings settings)
    {
        return ApplyScrubbers.Apply(input, settings.instanceScrubbers)
            .Replace("\r\n", "\n");
    }

    static async Task VerifyExisting(string input, FilePair file, Func<FilePair, Task> diff)
    {
        var verifiedText = await FileHelpers.ReadText(file.Verified);
        verifiedText = verifiedText.Replace("\r\n", "\n");
        try
        {
            assert(verifiedText, input);
        }
        catch (Exception exception)
            when (!BuildServerDetector.Detected)
        {
            throw await VerificationException(diff, notEqual: file, message: exception.Message);
        }
    }

    static async Task VerifyFirstTime(FilePair file, Func<FilePair, Task> launchDiff)
    {
        throw await VerificationException(launchDiff, file);
    }
}