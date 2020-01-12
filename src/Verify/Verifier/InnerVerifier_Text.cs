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

        var result = await Comparer.Text(file, scrubbedInput);
        innerVerifier.HandleCompareResult(result, file);
        await innerVerifier.ThrowIfRequired();
    }

    static string ScrubInput(string input, VerifySettings settings)
    {
        return ApplyScrubbers.Apply(input, settings.instanceScrubbers)
            .Replace("\r\n", "\n");
    }
}