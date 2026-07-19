// The no-match pass is the dominant real case: scrubbers are registered globally,
// but most documents (and most serialized property values) contain nothing they
// match. Those passes should allocate nothing.
// Inline_* registers a Replace whose find never occurs. Line_* registers a
// RemoveLinesContaining whose needle never occurs, which routes through the line
// phase. Both_* registers both.
// Allocated is the metric that matters here, not Mean.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class NoMatchAllocationBenchmarks
{
    static Dictionary<string, object> emptyContext = [];

    string small = null!;
    string medium = null!;
    string large = null!;

    EngineScrubberSet inlineSet = null!;
    EngineScrubberSet lineSet = null!;
    EngineScrubberSet bothSet = null!;

    // One Counter for the whole run: Counter.Start allocates several dictionaries,
    // which would swamp the engine allocation being measured here. A verify creates
    // one Counter and scrubs many values through it.
    Counter counter = null!;

    [GlobalCleanup]
    public void Cleanup() => counter.Dispose();

    [GlobalSetup]
    public void Setup()
    {
        counter = Counter.Start();
        small = Build(260);     // p50
        medium = Build(2_900);  // p90
        large = Build(31_000);  // p99

        inlineSet = EngineScrubberSet.ForScrubbers([Scrubber.Replace("NEVER_OCCURS_TOKEN", "{X}")]);
        lineSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "NEVER_OCCURS_TOKEN")]);
        bothSet = EngineScrubberSet.ForScrubbers(
        [
            Scrubber.Replace("NEVER_OCCURS_TOKEN", "{X}"),
            Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "ALSO_NEVER")
        ]);
    }

    static string Build(int targetChars)
    {
        var builder = new StringBuilder();
        while (builder.Length < targetChars)
        {
            builder.Append("  \"name\": \"some value here\",\n");
        }

        return builder.ToString();
    }

    string Run(string input, EngineScrubberSet set) =>
        ScrubEngine.Run(input, set, counter, emptyContext, applyDirectoryReplacements: false);

    [Benchmark]
    public string Inline_Small() => Run(small, inlineSet);

    [Benchmark]
    public string Inline_Medium() => Run(medium, inlineSet);

    [Benchmark]
    public string Inline_Large() => Run(large, inlineSet);

    [Benchmark]
    public string Line_Small() => Run(small, lineSet);

    [Benchmark]
    public string Line_Medium() => Run(medium, lineSet);

    [Benchmark]
    public string Line_Large() => Run(large, lineSet);

    [Benchmark]
    public string Both_Large() => Run(large, bothSet);
}
