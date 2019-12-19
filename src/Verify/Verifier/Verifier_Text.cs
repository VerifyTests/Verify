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
        var (receivedPath, verifiedPath) = GetFileNames(extension, settings.Namer);

        Guard.AgainstNull(input, nameof(input));
        input = ApplyScrubbers.Apply(input, settings.instanceScrubbers);
        input = input.Replace("\r\n", "\n");
        FileHelpers.DeleteIfEmpty(verifiedPath);
        if (File.Exists(verifiedPath))
        {
            return VerifyExisting(input, verifiedPath, receivedPath, extension);
        }
        return VerifyFirstTime(input, receivedPath, verifiedPath, extension);
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
            await ClipboardCapture.Append(receivedPath, verifiedPath);
            if (DiffTools.TryGetTextDiff(extension, out var diffTool))
            {
                DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
            }

            throw exceptionBuilder($@"Verification command has been copied to the clipboard.
{exception.Message}");
        }
    }

    static async Task VerifyFirstTime(string input, string receivedPath, string verifiedPath, string extension)
    {
        if (!BuildServerDetector.Detected)
        {
            await FileHelpers.WriteText(receivedPath, input);
            await ClipboardCapture.Append(receivedPath, verifiedPath);
            if (DiffTools.TryGetTextDiff(extension, out var diffTool))
            {
                FileHelpers.WriteEmptyText(verifiedPath);
                DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
            }
        }

        throw VerificationNotFoundException(verifiedPath, exceptionBuilder);
    }
}
