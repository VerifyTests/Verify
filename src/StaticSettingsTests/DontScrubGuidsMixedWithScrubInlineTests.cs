public class DontScrubGuidsMixedWithScrubInlineTests :
    BaseTest
{
    public DontScrubGuidsMixedWithScrubInlineTests() =>
        VerifierSettings.DontScrubGuids();

    [Fact]
    public Task ScrubInlineGuids() =>
        Verify(" {48cac197-20f2-4e16-8959-1c6a38090e0d} ")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubGuidsAndScrubInlineGuids() =>
        Verify(" {48cac197-20f2-4e16-8959-1c6a38090e0d} ")
            .ScrubGuids()
            .ScrubInlineGuids();
}