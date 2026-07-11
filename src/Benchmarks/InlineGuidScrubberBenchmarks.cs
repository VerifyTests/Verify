// Calibrated from a scan of 33,707 *.verified.* text files across D:\Code (2026-07):
// size p50=260 chars, p90=2.9KB, p99=31KB. Guid placeholders appear in 5.1% of files
// (density p50 1.5, p90 23 per 1000 chars; the densest single file held 1,057).
// GuidScrubber.ReplaceGuids walks StringBuilder.GetChunks() testing every 36-char
// window, so the scan cost is paid on every scrubbed file even when no guid is present
// (the common case). Builders produced by serialization span many chunks and exercise
// the cross-chunk carryover branch (recently bug-fixed); builders from raw-string
// verification are a single chunk. Both shapes are covered.
// A fresh Counter is created per invocation to mirror one-Counter-per-verify; its
// allocation (a handful of empty dictionaries) is a constant floor across all rows.
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

    [GlobalSetup]
    public void Setup()
    {
        smallNone = Build(260, 0);        // p50
        mediumNone = Build(2_900, 0);     // p90
        largeNone = Build(31_000, 0);     // p99
        mediumTypical = Build(2_900, 12); // ~1 guid / 12 lines ~= p50 density
        largeTypical = Build(31_000, 12);
        largeDense = Build(31_000, 1);    // a guid on every line - worst case
    }

    // Lines of ~40 chars; every guidEveryLines'th line embeds a distinct, quote-delimited
    // guid in canonical "D" format so GuidScrubber.ReplaceGuids parses and replaces it.
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

    // Rebuilds the same content as many ~200-char chunks, the way an incrementally
    // appended serialization buffer arrives, forcing the chunk-carryover branch.
    static StringBuilder MultiChunk(string content)
    {
        var builder = new StringBuilder();
        for (var start = 0; start < content.Length; start += 200)
        {
            var length = Math.Min(200, content.Length - start);
            builder.Append(content.AsSpan(start, length));
        }

        return builder;
    }

    static void Scrub(StringBuilder builder) =>
        GuidScrubber.ReplaceGuids(builder, Counter.Start());

    // Single chunk (raw-string verification shape)

    [Benchmark(Baseline = true)]
    public void Small_None() => Scrub(new(smallNone));

    [Benchmark]
    public void Medium_None() => Scrub(new(mediumNone));

    [Benchmark]
    public void Large_None() => Scrub(new(largeNone));

    [Benchmark]
    public void Medium_Typical() => Scrub(new(mediumTypical));

    [Benchmark]
    public void Large_Typical() => Scrub(new(largeTypical));

    [Benchmark]
    public void Large_Dense() => Scrub(new(largeDense));

    // Multi chunk (serialized-output shape) - exercises the cross-chunk carryover branch

    [Benchmark]
    public void Large_MultiChunk_None() => Scrub(MultiChunk(largeNone));

    [Benchmark]
    public void Large_MultiChunk_Typical() => Scrub(MultiChunk(largeTypical));
}
