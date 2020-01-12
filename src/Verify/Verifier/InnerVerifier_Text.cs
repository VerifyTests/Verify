using System;
using System.IO;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public async Task Verify(string input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        var extension = settings.ExtensionOrTxt();
        var innerVerifier = new VerifyEngine(
            extension,
            settings,
            testType,
            directory,
            testName);

        var file = GetFileNames(extension, settings.Namer);

        var scrubbedInput = ScrubInput(input, settings);
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (!File.Exists(file.Verified))
        {
            await FileHelpers.WriteText(file.Received, input);
            innerVerifier.AddMissing(file);
        }
        else
        {
            var verifiedText = await FileHelpers.ReadText(file.Verified);
            verifiedText = verifiedText.Replace("\r\n", "\n");
            if (!string.Equals(verifiedText, scrubbedInput, StringComparison.OrdinalIgnoreCase))
            {
                await FileHelpers.WriteText(file.Received, scrubbedInput);
                innerVerifier.AddNotEquals(file);
            }
        }

        await innerVerifier.ThrowIfRequired();
    }

    static string ScrubInput(string input, VerifySettings settings)
    {
        return ApplyScrubbers.Apply(input, settings.instanceScrubbers)
            .Replace("\r\n", "\n");
    }
}