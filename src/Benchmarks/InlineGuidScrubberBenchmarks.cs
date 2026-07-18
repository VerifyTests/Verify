// Calibrated from a scan of 33,707 *.verified.* text files across D:\Code (2026-07):
// size p50=260 chars, p90=2.9KB, p99=31KB. Guid placeholders appear in 5.1% of files
// (density p50 1.5, p90 23 per 1000 chars; the densest single file held 1,057).
// Legacy_* rows run the pre-engine chunk-walking implementation (LegacyScrubbers.cs);
// Engine_* rows run the span engine. The Composition rows show the whole-pipeline win:
// guids + dates + line removal as one engine pass vs sequential legacy passes.
// A fresh Counter is created per invocation to mirror one-Counter-per-verify.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class InlineGuidScrubberBenchmarks
{
    string smallNone = null!;
    string mediumNone = null!;
    string largeNone = null!;
    string mediumTypical = null!;
    string largeTypical = null!;
    string largeDense = null!;
    string composition = null!;

    EngineScrubberSet guidSet = null!;
    EngineScrubberSet compositionSet = null!;
    static Dictionary<string, object> emptyContext = [];
    const string isoFormat = "yyyy-MM-ddTHH:mm:ss";

    [GlobalSetup]
    public void Setup()
    {
        smallNone = Build(260, 0);        // p50
        mediumNone = Build(2_900, 0);     // p90
        largeNone = Build(31_000, 0);     // p99
        mediumTypical = Build(2_900, 12); // ~1 guid / 12 lines ~= p50 density
        largeTypical = Build(31_000, 12);
        largeDense = Build(31_000, 1);    // a guid on every line - worst case
        composition = BuildComposition(31_000);

        guidSet = EngineScrubberSet.ForScrubbers([GuidMatcher.Instance]);
        List<Scrubber> compositionScrubbers = [GuidMatcher.Instance, Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "SECRET")];
        compositionScrubbers.AddRange(DateMatchers.DateTimes(isoFormat, null));
        compositionSet = EngineScrubberSet.ForScrubbers(compositionScrubbers);
    }

    // Lines of ~40 chars; every guidEveryLines'th line embeds a distinct, quote-delimited
    // guid in canonical "D" format.
    static string Build(int targetChars, int guidEveryLines)
    {
        var builder = new StringBuilder();
        var line = 0;
        var guidSeed = 1;
        while (builder.Length < targetChars)
        {
            if (guidEveryLines > 0 &&
                line % guidEveryLines == 0)
            {
                builder.Append("  \"id\": \"");
                builder.Append($"{guidSeed:x8}-0000-4000-8000-000000000000");
                builder.Append("\",");
                guidSeed++;
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

    // Mixed content: a guid every 12 lines, an ISO timestamp every 4, a SECRET line every 10
    static string BuildComposition(int targetChars)
    {
        var builder = new StringBuilder();
        var line = 0;
        var seed = 1;
        var baseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        while (builder.Length < targetChars)
        {
            if (line % 12 == 0)
            {
                builder.Append("  \"id\": \"");
                builder.Append($"{seed:x8}-0000-4000-8000-000000000000");
                builder.Append("\",");
            }
            else if (line % 10 == 0)
            {
                builder.Append("  [trace] SECRET session token for request ");
                builder.Append(line);
            }
            else if (line % 4 == 0)
            {
                builder.Append("  \"timestamp\": \"");
                builder.Append(baseDate.AddSeconds(seed).ToString(isoFormat, Culture.InvariantCulture));
                builder.Append("\",");
            }
            else
            {
                builder.Append("  \"name\": \"item ");
                builder.Append(line);
                builder.Append(" description text\",");
            }

            seed++;
            builder.Append('\n');
            line++;
        }

        return builder.ToString();
    }

    static void LegacyScrub(string content)
    {
        using var counter = Counter.Start();
        var builder = new StringBuilder(content);
        LegacyGuidScrubber.ReplaceGuids(builder, counter);
    }

    string EngineScrub(string content)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(content, guidSet, counter, emptyContext, applyDirectoryReplacements: false);
    }

    [Benchmark(Baseline = true)]
    public void Legacy_Small_None() => LegacyScrub(smallNone);

    [Benchmark]
    public void Legacy_Medium_None() => LegacyScrub(mediumNone);

    [Benchmark]
    public void Legacy_Large_None() => LegacyScrub(largeNone);

    [Benchmark]
    public void Legacy_Medium_Typical() => LegacyScrub(mediumTypical);

    [Benchmark]
    public void Legacy_Large_Typical() => LegacyScrub(largeTypical);

    [Benchmark]
    public void Legacy_Large_Dense() => LegacyScrub(largeDense);

    [Benchmark]
    public string Engine_Small_None() => EngineScrub(smallNone);

    [Benchmark]
    public string Engine_Medium_None() => EngineScrub(mediumNone);

    [Benchmark]
    public string Engine_Large_None() => EngineScrub(largeNone);

    [Benchmark]
    public string Engine_Medium_Typical() => EngineScrub(mediumTypical);

    [Benchmark]
    public string Engine_Large_Typical() => EngineScrub(largeTypical);

    [Benchmark]
    public string Engine_Large_Dense() => EngineScrub(largeDense);

    // Whole-pipeline comparison: three scrubbers over mixed content

    [Benchmark]
    public void Legacy_Composition()
    {
        using var counter = Counter.Start();
        var builder = new StringBuilder(composition);
        LegacyGuidScrubber.ReplaceGuids(builder, counter);
        LegacyDateScrubber.ReplaceDateTimes(builder, isoFormat, counter, Culture.CurrentCulture);
        builder.RemoveLinesContaining(StringComparison.Ordinal, "SECRET");
    }

    [Benchmark]
    public string Engine_Composition()
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(composition, compositionSet, counter, emptyContext, applyDirectoryReplacements: false);
    }
}
