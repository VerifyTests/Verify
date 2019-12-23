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
            return VerifyExisting(input, file.Verified, file.Received, extension);
        }
        return VerifyFirstTime(input, file.Received, file.Verified, extension);
    }

    static async Task VerifyExisting(string input, string verifiedPath, string receivedPath, string extension)
    {
        var verifiedText = await FileHelpers.ReadText(verifiedPath);
        verifiedText = verifiedText.Replace("\r\n", "\n");
        try
        {
            assert(verifiedText, input);
        }
        catch (Exception exception)
            when (!BuildServerDetector.Detected)
        {
            await FileHelpers.WriteText(receivedPath, input);
            await ClipboardCapture.AppendMove(receivedPath, verifiedPath);
            if (DiffTools.TryGetTextDiff(extension, out var diffTool))
            {
                DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
            }

            throw VerificationException(notEqual: receivedPath, message: exception.Message);
        }
    }

    static async Task VerifyFirstTime(string input, string receivedPath, string verifiedPath, string extension)
    {
        if (!BuildServerDetector.Detected)
        {
            await FileHelpers.WriteText(receivedPath, input);
            await ClipboardCapture.AppendMove(receivedPath, verifiedPath);
            if (DiffTools.TryGetTextDiff(extension, out var diffTool))
            {
                FileHelpers.WriteEmptyText(verifiedPath);
                DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
            }
        }

        throw VerificationException(verifiedPath);
    }
}
