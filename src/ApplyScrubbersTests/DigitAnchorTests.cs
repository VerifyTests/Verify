// The digit anchor lets the engine jump between digit positions instead of
// probing every one. It may only be used when the format really does render an
// ASCII digit first, otherwise candidate windows are never probed.
public class DigitAnchorTests
{
    [Fact]
    public void LeadingOptionalFractionIsScrubbed()
    {
        // Upper case F renders nothing when the fraction is zero, so the value
        // starts with the literal space and a digit anchor would never probe it
        var rendered = new DateTime(2020, 1, 1, 0, 30, 0).ToString("FF mm", CultureInfo.InvariantCulture);
        Assert.Equal(" 30", rendered);

        using var counter = Counter.Start();
        var scrubbers = DateMatchers.DateTimes("FF mm", CultureInfo.InvariantCulture);
        Assert.Equal("[DateTime_1]", EngineRunner.Run($"[{rendered}]", counter, scrubbers));
    }

    [Fact]
    public void NonZeroFractionIsScrubbed()
    {
        var date = new DateTime(2020, 1, 1, 0, 30, 0).AddMilliseconds(250);
        var rendered = date.ToString("FF mm", CultureInfo.InvariantCulture);
        Assert.Equal("25 30", rendered);

        using var counter = Counter.Start();
        var scrubbers = DateMatchers.DateTimes("FF mm", CultureInfo.InvariantCulture);
        Assert.Equal("[DateTime_1]", EngineRunner.Run($"[{rendered}]", counter, scrubbers));
    }

    [Fact]
    public void DigitLeadingFormatIsScrubbed()
    {
        using var counter = Counter.Start();
        var scrubbers = DateMatchers.DateTimes("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Assert.Equal("[DateTime_1]", EngineRunner.Run("[2024-01-02]", counter, scrubbers));
    }

    [Fact]
    public void NameLeadingFormatIsScrubbed()
    {
        using var counter = Counter.Start();
        var scrubbers = DateMatchers.DateTimes("MMMM d yyyy", CultureInfo.InvariantCulture);
        Assert.Equal("[DateTime_1]", EngineRunner.Run("[January 2 2024]", counter, scrubbers));
    }

    [Fact]
    public void LowerCaseFractionIsScrubbed()
    {
        // Lower case f always renders digits, so the anchor stays available
        var rendered = new DateTime(2020, 1, 1, 0, 30, 0).ToString("ff mm", CultureInfo.InvariantCulture);
        Assert.Equal("00 30", rendered);

        using var counter = Counter.Start();
        var scrubbers = DateMatchers.DateTimes("ff mm", CultureInfo.InvariantCulture);
        Assert.Equal("[DateTime_1]", EngineRunner.Run($"[{rendered}]", counter, scrubbers));
    }
}
