public class ScrubInlineDateTimesTests :
    BaseTest
{
    public ScrubInlineDateTimesTests() =>
        VerifierSettings.ScrubInlineDateTimes("yyyyMMdd");

    [Fact]
    public Task DontScrubDateTimesShouldOverrideScrubInlineDateTimes() =>
        Verify("ABEF20250203485CD").DontScrubDateTimes();
}