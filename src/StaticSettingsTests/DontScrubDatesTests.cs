public class DontScrubDatesTests :
    BaseTest
{
    public DontScrubDatesTests() =>
        VerifierSettings.DontScrubDateTimes();

    [Fact]
    public async Task ScrubInlineDates()
    {
        var settings = Verify(" 2020/10/01 ");
        await Throws(() => settings.ScrubInlineDates("yyyy/MM/dd"))
            .IgnoreStackTrace();
    }

    [Fact]
    public async Task ScrubInlineDateTimes()
    {
        var settings = Verify(" 2020/10/01 ");
        await Throws(() => settings.ScrubInlineDateTimes("yyyy/MM/dd"))
            .IgnoreStackTrace();
    }

    [Fact]
    public async Task ScrubInlineDateTimeOffsets()
    {
        var settings = Verify(" 2020/10/01 ");
        await Throws(() => settings.ScrubInlineDateTimeOffsets("yyyy/MM/dd"))
            .IgnoreStackTrace();
    }

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