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
        var file = GetFileNames(extension, settings.Namer);

        Guard.AgainstNull(input, nameof(input));
        input = ApplyScrubbers.Apply(input, settings.instanceScrubbers);
        input = input.Replace("\r\n", "\n");
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (File.Exists(file.Verified))
        {
            return VerifyExisting(input, file);
        }
        return VerifyFirstTime(input, file);
    }

    static async Task VerifyExisting(string input, FilePair file)
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
            await FileHelpers.WriteText(file.Received, input);
            await ClipboardCapture.AppendMove(file.Received, file.Verified);
            if (DiffTools.TryGetTextDiff(file.Extension, out var diffTool))
            {
                DiffRunner.Launch(diffTool, file.Received, file.Verified);
            }

            throw VerificationException(notEqual: file.Received, message: exception.Message);
        }
    }

    static async Task VerifyFirstTime(string input, FilePair file)
    {
        if (!BuildServerDetector.Detected)
        {
            await FileHelpers.WriteText(file.Received, input);
            await ClipboardCapture.AppendMove(file.Received, file.Verified);
            if (DiffTools.TryGetTextDiff(file.Extension, out var diffTool))
            {
                FileHelpers.WriteEmptyText(file.Verified);
                DiffRunner.Launch(diffTool, file.Received, file.Verified);
            }
        }

        throw VerificationException(file);
    }
}
