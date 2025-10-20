public class DontScrubGuidsTests :
    BaseTest
{
    public DontScrubGuidsTests() =>
        VerifierSettings.DontScrubGuids();

    [Fact]
    public async Task ScrubInlineGuids()
    {
        var settings = Verify(" 48cac197-20f2-4e16-8959-1c6a38090e0d ");
        await Throws(() => settings.ScrubInlineGuids())
            .IgnoreStackTrace();
    }

    [Fact]
    public Task ScrubGuidsAndScrubInlineGuids() =>
        Verify(" 48cac197-20f2-4e16-8959-1c6a38090e0d ")
            .ScrubGuids()
            .ScrubInlineGuids();
}