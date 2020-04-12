using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public async Task Verify(string input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        var extension = settings.ExtensionOrTxt();
        var engine = new VerifyEngine(
            extension,
            settings,
            testType,
            directory,
            testName);

        var file = GetFileNames(extension, settings.Namer);

        var scrubbedInput = ApplyScrubbers.Apply(input, settings.instanceScrubbers);

        var result = await Comparer.Text(file, scrubbedInput);
        engine.HandleCompareResult(result, file);
        await engine.ThrowIfRequired();
    }
}