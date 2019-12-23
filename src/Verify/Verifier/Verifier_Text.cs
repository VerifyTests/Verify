using System;
using System.IO;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    public Task Verify(string input, VerifySettings? settings = null)
    {
        settings = settings.OrDefault();

        var extension = settings.ExtensionOrTxt();

        async Task Diff(FilePair pair)
        {
            await FileHelpers.WriteText(pair.Received, input);
            if (DiffTools.TryGetTextDiff(extension, out var diffTool))
            {
                FileHelpers.WriteEmptyText(pair.Verified);
                DiffRunner.Launch(diffTool, pair.Received, pair.Verified);
            }
        }

        var file = GetFileNames(extension, settings.Namer);

        Guard.AgainstNull(input, nameof(input));
        input = ApplyScrubbers.Apply(input, settings.instanceScrubbers);
        input = input.Replace("\r\n", "\n");
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (File.Exists(file.Verified))
        {
            return VerifyExisting(input, file, Diff);
        }

        return VerifyFirstTime(file, Diff);
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

    static async Task VerifyFirstTime(FilePair file, Func<FilePair, Task> diff)
    {
        throw await VerificationException(diff, file);
    }
}