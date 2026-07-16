// Calibrated from a scan of 33,707 *.verified.* text files across D:\Code (2026-07).
// DateTime placeholders are the single most common scrubber target (14.6% of files,
// density p50 6.5 per 1000 chars); DateTimeOffset 2.3%, DateOnly 1.6%. File sizes:
// p50=260 chars, p90=2.9KB, p99=31KB.
//
// DateScrubber ToString()s the whole builder (via AsSpan) and then tests every window
// of the format's length, so the scan cost dominates and is paid whether or not a date
// is present. A fixed-length format (ISO "yyyy-MM-ddTHH:mm:ss", min==max) takes the
// ReplaceFixedLength path; a variable-length format (short-date "d", min<max) takes the
// ReplaceVariableLength path, which probes several window lengths per position and is
// materially more expensive. The *_None rows feed identical no-match input to the fixed
// and variable scrubbers so the two scan strategies can be compared directly.
// Tokens are formatted with the same format+culture the scrubber parses with, so they
// round-trip regardless of the host culture. A fresh Counter is created per invocation.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class InlineDateScrubberBenchmarks
{
    const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    const string dateFormat = "d";
    const string dateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:sszzz";

    static readonly CultureInfo culture = CultureInfo.CurrentCulture;
    static readonly DateTime baseDateTime = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    Action<StringBuilder, Counter> dateTimeScrubber = null!;
    Action<StringBuilder, Counter> dateScrubber = null!;
    Action<StringBuilder, Counter> dateTimeOffsetScrubber = null!;

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
        dateTimeScrubber = DateScrubber.BuildDateTimeScrubber(dateTimeFormat, null);
        dateScrubber = DateScrubber.BuildDateScrubber(dateFormat, null);
        dateTimeOffsetScrubber = DateScrubber.BuildDateTimeOffsetScrubber(dateTimeOffsetFormat, null);

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

    // DateTime, fixed-length (ISO) path

    [Benchmark(Baseline = true)]
    public void DateTime_Small_None() => dateTimeScrubber(new(smallNone), Counter.Start());

    [Benchmark]
    public void DateTime_Medium_None() => dateTimeScrubber(new(mediumNone), Counter.Start());

    [Benchmark]
    public void DateTime_Large_None() => dateTimeScrubber(new(largeNone), Counter.Start());

    [Benchmark]
    public void DateTime_Medium_Typical() => dateTimeScrubber(new(dateTimeMediumTypical), Counter.Start());

    [Benchmark]
    public void DateTime_Large_Typical() => dateTimeScrubber(new(dateTimeLargeTypical), Counter.Start());

    // DateOnly, variable-length (short-date) path - same no-match input as DateTime_Medium_None

    [Benchmark]
    public void Date_Medium_None() => dateScrubber(new(mediumNone), Counter.Start());

    [Benchmark]
    public void Date_Medium_Typical() => dateScrubber(new(dateMediumTypical), Counter.Start());

    // DateTimeOffset

    [Benchmark]
    public void DateTimeOffset_Medium_Typical() => dateTimeOffsetScrubber(new(dateTimeOffsetMediumTypical), Counter.Start());
}
