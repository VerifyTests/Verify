public class DontScrubDatesMixedWithScrubInlineTests :
    BaseTest
{
    public DontScrubDatesMixedWithScrubInlineTests() =>
        VerifierSettings.DontScrubDateTimes();

    [Fact]
    public Task ScrubInlineDates() =>
        Verify(" 2020/10/01 ")
            .ScrubInlineDates("yyyy/MM/dd");

    [Fact]
    public Task ScrubInlineDateTimes() =>
        Verify(" 2020/10/01 ")
            .ScrubInlineDateTimes("yyyy/MM/dd");

    [Fact]
    public Task ScrubInlineDateTimeOffsets() =>
        Verify(" 2020/10/01 ")
            .ScrubInlineDateTimeOffsets("yyyy/MM/dd");

    [Fact]
    public Task ScrubDatesAndScrubInlineDateTimes() =>
        Verify(" 2020/10/01 ")
            .ScrubDateTimes()
            .ScrubInlineDateTimes("yyyy/MM/dd");

    [Fact]
    public Task ScrubDatesAndScrubInlineDateTimeOffsets() =>
        Verify(" 2020/10/01 ")
            .ScrubDateTimes()
            .ScrubInlineDateTimeOffsets("yyyy/MM/dd");

    [Fact]
    public Task ScrubDatesAndScrubInlineDates() =>
        Verify(" 2020/10/01 ")
            .ScrubDateTimes()
            .ScrubInlineDates("yyyy/MM/dd");
}