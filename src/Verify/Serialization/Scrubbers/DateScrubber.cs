// Legacy entry points retained so existing direct callers keep compiling.
// New code should construct a DateTimePatternScrubber/DateTimeOffsetPatternScrubber/DatePatternScrubber
// and register it via VerifierSettings.AddScrubber.

static class DateScrubber
{
    public static void ReplaceDateTimes(StringBuilder builder, string format, Counter counter, Culture culture) =>
        PatternScrubberRunner.Run(builder, new DateTimePatternScrubber(format, culture), counter);

    public static void ReplaceDateTimeOffsets(StringBuilder builder, string format, Counter counter, Culture culture) =>
        PatternScrubberRunner.Run(builder, new DateTimeOffsetPatternScrubber(format, culture), counter);

#if NET6_0_OR_GREATER

    public static void ReplaceDates(StringBuilder builder, string format, Counter counter, Culture culture) =>
        PatternScrubberRunner.Run(builder, new DatePatternScrubber(format, culture), counter);

#endif
}
