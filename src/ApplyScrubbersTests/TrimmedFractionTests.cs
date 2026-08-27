// Formats ending in an upper case fraction produce a second scrubber for the trimmed
// format, since those fractions render as empty when zero. A one character format
// string is a standard format specifier, so the trimmed format has to be escaped.
public class TrimmedFractionTests
{
    static readonly CultureInfo enUs = new("en-US");

    [Fact]
    public void SingleCharTrimmedFormatDoesNotBecomeStandardFormat()
    {
        var scrubbers = DateMatchers.DateTimes("s.F", enUs);

        // "s" as a standard format is the sortable pattern, so the trimmed scrubber
        // used to swallow every full sortable date-time in the output
        var result = EngineRunner.Run("2020-01-01T10:20:30", scrubbers);
        Assert.NotEqual("DateTime_1", result);

        // the seconds the format actually asks for still scrub
        Assert.Equal("DateTime_1", EngineRunner.Run("9", scrubbers));
    }

    [Fact]
    public void SingleCharTrimmedFormatIsNotRejected()
    {
        // "H" is not a standard format specifier, so trimming used to throw
        // "Invalid format: H" at registration
        var scrubbers = DateMatchers.DateTimes("H.F", enUs);

        Assert.Equal(2, scrubbers.Length);
        Assert.Equal("DateTime_1", EngineRunner.Run("9", scrubbers));
    }

    [Fact]
    public void FractionOnlyFormatHasNoTrimmedScrubber()
    {
        // trimming leaves nothing to parse
        var scrubbers = DateMatchers.DateTimes(".F", enUs);

        Assert.Single(scrubbers);
    }
}
