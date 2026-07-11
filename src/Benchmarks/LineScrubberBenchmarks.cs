// Calibrated from a scan of 33,707 *.verified.* text files across D:\Code (2026-07):
// line counts p50=15, p90=88, p99=508. ScrubLinesContaining is the most common hand-written
// scrubber in the wild (56 call sites over 15 repos, 1-2 needles, OrdinalIgnoreCase default).
// Legacy_* rows run the pre-engine implementation: whole-document ToString + StringReader
// (one string allocation per line) + rebuild. Engine_* rows run the span engine line phase
// (per-line span checks; kept lines never allocate).
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class LineScrubberBenchmarks
{
    string small = null!;
    string medium = null!;
    string large = null!;
    string mediumWithBlanks = null!;

    EngineScrubberSet containsOrdinalSet = null!;
    EngineScrubberSet containsIgnoreCaseSet = null!;
    EngineScrubberSet emptyLinesSet = null!;
    EngineScrubberSet replaceLinesSet = null!;
    static Dictionary<string, object> emptyContext = [];

    [GlobalSetup]
    public void Setup()
    {
        small = Build(15, false);   // p50
        medium = Build(88, false);  // p90
        large = Build(508, false);  // p99
        mediumWithBlanks = Build(88, true);

        containsOrdinalSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "SECRET")]);
        containsIgnoreCaseSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLinesContaining("secret")]);
        emptyLinesSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveEmptyLines()]);
        replaceLinesSet = EngineScrubberSet.ForScrubbers([Scrubber.ReplaceLines(Keep)]);
    }

    // Lines of ~55 chars; every 10th line carries a "SECRET" marker (the kind of noise
    // line ScrubLinesContaining removes). withBlanks intersperses blank lines for
    // ScrubEmptyLines.
    static string Build(int lineCount, bool withBlanks)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < lineCount; i++)
        {
            if (withBlanks &&
                i % 5 == 0)
            {
                builder.Append('\n');
                continue;
            }

            if (i % 10 == 0)
            {
                builder.Append("  [trace] SECRET session token for request ");
                builder.Append(i);
            }
            else
            {
                builder.Append("  [info] processed item ");
                builder.Append(i);
                builder.Append(" in 12ms, status ok, worker idle");
            }

            builder.Append('\n');
        }

        return builder.ToString();
    }

    static string? Keep(string line) => line;

    string Engine(EngineScrubberSet set, string content)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(content, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    // ScrubLinesContaining - Contains match on every line

    [Benchmark(Baseline = true)]
    public void Legacy_RemoveLinesContaining_Small() =>
        new StringBuilder(small).RemoveLinesContaining(StringComparison.Ordinal, "SECRET");

    [Benchmark]
    public void Legacy_RemoveLinesContaining_Medium() =>
        new StringBuilder(medium).RemoveLinesContaining(StringComparison.Ordinal, "SECRET");

    [Benchmark]
    public void Legacy_RemoveLinesContaining_Large() =>
        new StringBuilder(large).RemoveLinesContaining(StringComparison.Ordinal, "SECRET");

    [Benchmark]
    public string Engine_RemoveLinesContaining_Small() =>
        Engine(containsOrdinalSet, small);

    [Benchmark]
    public string Engine_RemoveLinesContaining_Medium() =>
        Engine(containsOrdinalSet, medium);

    [Benchmark]
    public string Engine_RemoveLinesContaining_Large() =>
        Engine(containsOrdinalSet, large);

    // OrdinalIgnoreCase is the wild default (44 of 56 call sites)

    [Benchmark]
    public void Legacy_RemoveLinesContaining_Large_IgnoreCase() =>
        new StringBuilder(large).RemoveLinesContaining(StringComparison.OrdinalIgnoreCase, "secret");

    [Benchmark]
    public string Engine_RemoveLinesContaining_Large_IgnoreCase() =>
        Engine(containsIgnoreCaseSet, large);

    // ScrubEmptyLines

    [Benchmark]
    public void Legacy_RemoveEmptyLines_Medium() =>
        new StringBuilder(mediumWithBlanks).RemoveEmptyLines();

    [Benchmark]
    public string Engine_RemoveEmptyLines_Medium() =>
        Engine(emptyLinesSet, mediumWithBlanks);

    // ScrubLinesWithReplace - identity replace isolates the read/rewrite round-trip

    [Benchmark]
    public void Legacy_ReplaceLines_Medium() =>
        new StringBuilder(medium).ReplaceLines(Keep);

    [Benchmark]
    public string Engine_ReplaceLines_Medium() =>
        Engine(replaceLinesSet, medium);
}
