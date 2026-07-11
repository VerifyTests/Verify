// Calibrated from a scan of 33,707 *.verified.* text files across D:\Code (2026-07):
// line counts p50=15, p90=88, p99=508. The line scrubbers round-trip the whole builder
// through StringReader.ReadLine (one string allocation per line) and rebuild it, so cost
// and allocations scale with line count rather than raw byte size.
// FilterLines (the predicate path used by ScrubLines) is covered by FilterLinesBenchmarks;
// this covers the Contains-matching (ScrubLinesContaining), empty-line (ScrubEmptyLines)
// and replace (ScrubLinesWithReplace) paths.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class LineScrubberBenchmarks
{
    string small = null!;
    string medium = null!;
    string large = null!;
    string mediumWithBlanks = null!;

    [GlobalSetup]
    public void Setup()
    {
        small = Build(15, false);   // p50
        medium = Build(88, false);  // p90
        large = Build(508, false);  // p99
        mediumWithBlanks = Build(88, true);
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

    // ScrubLinesContaining - Contains match on every line

    [Benchmark(Baseline = true)]
    public void RemoveLinesContaining_Small() =>
        new StringBuilder(small).RemoveLinesContaining(StringComparison.Ordinal, "SECRET");

    [Benchmark]
    public void RemoveLinesContaining_Medium() =>
        new StringBuilder(medium).RemoveLinesContaining(StringComparison.Ordinal, "SECRET");

    [Benchmark]
    public void RemoveLinesContaining_Large() =>
        new StringBuilder(large).RemoveLinesContaining(StringComparison.Ordinal, "SECRET");

    // ScrubEmptyLines

    [Benchmark]
    public void RemoveEmptyLines_Medium() =>
        new StringBuilder(mediumWithBlanks).RemoveEmptyLines();

    // ScrubLinesWithReplace - identity replace isolates the read/rewrite round-trip

    [Benchmark]
    public void ReplaceLines_Medium() =>
        new StringBuilder(medium).ReplaceLines(Keep);
}
