// Calibrated from a scan of 33,707 *.verified.* text files across D:\Code (2026-07).
// DateTime placeholders are the single most common scrubber target (14.6% of files,
// density p50 6.5 per 1000 chars); DateTimeOffset 2.3%, DateOnly 1.6%. File sizes:
// p50=260 chars, p90=2.9KB, p99=31KB.
//
// Legacy_* rows run the pre-engine implementation (full builder.ToString() per pass, then a
// TryParseExact per window at every position). Engine_* rows run the span engine window
// scrubbers built by DateMatchers, which add a digit prefilter before each parse attempt.
// The ISO format is fixed length (single window size per position); the short-date "d"
// format is variable length (several window sizes per position, materially more expensive).
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class InlineDateScrubberBenchmarks
{
    const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    const string dateFormat = "d";
    const string dateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:sszzz";

    static readonly Culture culture = Culture.CurrentCulture;
    static readonly DateTime baseDateTime = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
    static Dictionary<string, object> emptyContext = [];

    Action<StringBuilder, Counter> legacyDateTimeScrubber = null!;
    Action<StringBuilder, Counter> legacyDateScrubber = null!;
    Action<StringBuilder, Counter> legacyDateTimeOffsetScrubber = null!;

    EngineScrubberSet dateTimeSet = null!;
    EngineScrubberSet dateSet = null!;
    EngineScrubberSet dateTimeOffsetSet = null!;

    // Plain no-match content shared across the fixed (DateTime) and variable (Date) paths
    string smallNone = null!;
    string mediumNone = null!;
    string largeNone = null!;

    string dateTimeMediumTypical = null!;
    string dateTimeLargeTypical = null!;
    string dateMediumTypical = null!;
    string dateTimeOffsetMediumTypical = null!;

    [GlobalSetup]
    public void Setup()
    {
        legacyDateTimeScrubber = LegacyDateScrubber.BuildDateTimeScrubber(dateTimeFormat, null);
        legacyDateScrubber = LegacyDateScrubber.BuildDateScrubber(dateFormat, null);
        legacyDateTimeOffsetScrubber = LegacyDateScrubber.BuildDateTimeOffsetScrubber(dateTimeOffsetFormat, null);

        dateTimeSet = EngineScrubberSet.ForScrubbers([.. DateMatchers.DateTimes(dateTimeFormat, null)]);
        dateSet = EngineScrubberSet.ForScrubbers([.. DateMatchers.Dates(dateFormat, null)]);
        dateTimeOffsetSet = EngineScrubberSet.ForScrubbers([.. DateMatchers.DateTimeOffsets(dateTimeOffsetFormat, null)]);

        smallNone = Build(260, 0, DateTimeToken);      // p50
        mediumNone = Build(2_900, 0, DateTimeToken);   // p90
        largeNone = Build(31_000, 0, DateTimeToken);   // p99

        // DateTime density ~ p50 (6.5 / 1000 chars) => ~1 token / 4 lines of ~40 chars
        dateTimeMediumTypical = Build(2_900, 4, DateTimeToken);
        dateTimeLargeTypical = Build(31_000, 4, DateTimeToken);
        // DateOnly / DateTimeOffset are rarer in the corpus, so sparser
        dateMediumTypical = Build(2_900, 8, DateToken);
        dateTimeOffsetMediumTypical = Build(2_900, 8, DateTimeOffsetToken);
    }

    static string DateTimeToken(int seed) =>
        baseDateTime.AddSeconds(seed).ToString(dateTimeFormat, culture);

    static string DateToken(int seed) =>
        DateOnly.FromDateTime(baseDateTime).AddDays(seed).ToString(dateFormat, culture);

    static string DateTimeOffsetToken(int seed) =>
        new DateTimeOffset(baseDateTime, TimeSpan.Zero).AddSeconds(seed).ToString(dateTimeOffsetFormat, culture);

    // Lines of ~40 chars; every tokenEveryLines'th line embeds a distinct, quote-delimited
    // date token that the matching scrubber parses and replaces.
    static string Build(int targetChars, int tokenEveryLines, Func<int, string> token)
    {
        var builder = new StringBuilder();
        var line = 0;
        var seed = 0;
        while (builder.Length < targetChars)
        {
            if (tokenEveryLines > 0 &&
                line % tokenEveryLines == 0)
            {
                builder.Append("  \"timestamp\": \"");
                builder.Append(token(seed));
                builder.Append("\",");
                seed++;
            }
            else
            {
                builder.Append("  \"name\": \"item ");
                builder.Append(line);
                builder.Append(" description text\",");
            }

            builder.Append('\n');
            line++;
        }

        return builder.ToString();
    }

    static void Legacy(Action<StringBuilder, Counter> scrubber, string content)
    {
        using var counter = Counter.Start();
        scrubber(new(content), counter);
    }

    static string Engine(EngineScrubberSet set, string content)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(content, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    // DateTime, fixed-length (ISO) path

    [Benchmark(Baseline = true)]
    public void Legacy_DateTime_Small_None() => Legacy(legacyDateTimeScrubber, smallNone);

    [Benchmark]
    public void Legacy_DateTime_Medium_None() => Legacy(legacyDateTimeScrubber, mediumNone);

    [Benchmark]
    public void Legacy_DateTime_Large_None() => Legacy(legacyDateTimeScrubber, largeNone);

    [Benchmark]
    public void Legacy_DateTime_Medium_Typical() => Legacy(legacyDateTimeScrubber, dateTimeMediumTypical);

    [Benchmark]
    public void Legacy_DateTime_Large_Typical() => Legacy(legacyDateTimeScrubber, dateTimeLargeTypical);

    [Benchmark]
    public string Engine_DateTime_Small_None() => Engine(dateTimeSet, smallNone);

    [Benchmark]
    public string Engine_DateTime_Medium_None() => Engine(dateTimeSet, mediumNone);

    [Benchmark]
    public string Engine_DateTime_Large_None() => Engine(dateTimeSet, largeNone);

    [Benchmark]
    public string Engine_DateTime_Medium_Typical() => Engine(dateTimeSet, dateTimeMediumTypical);

    [Benchmark]
    public string Engine_DateTime_Large_Typical() => Engine(dateTimeSet, dateTimeLargeTypical);

    // DateOnly, variable-length (short-date) path - same no-match input as DateTime_Medium_None

    [Benchmark]
    public void Legacy_Date_Medium_None() => Legacy(legacyDateScrubber, mediumNone);

    [Benchmark]
    public void Legacy_Date_Medium_Typical() => Legacy(legacyDateScrubber, dateMediumTypical);

    [Benchmark]
    public string Engine_Date_Medium_None() => Engine(dateSet, mediumNone);

    [Benchmark]
    public string Engine_Date_Medium_Typical() => Engine(dateSet, dateMediumTypical);

    // DateTimeOffset

    [Benchmark]
    public void Legacy_DateTimeOffset_Medium_Typical() => Legacy(legacyDateTimeOffsetScrubber, dateTimeOffsetMediumTypical);

    [Benchmark]
    public string Engine_DateTimeOffset_Medium_Typical() => Engine(dateTimeOffsetSet, dateTimeOffsetMediumTypical);
}
