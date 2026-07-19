// Splice removes and re-inserts in the shared chunk list, so every match shifts the
// elements after it. That is free while the list is short or matches land at the
// tail, but a second scrubber running over a list a first scrubber already
// fragmented splices near the front of a long list, giving O(matches x chunks).
// Fragmented_* rows are that shape: scrubber A (longer find, so it runs first)
// splits the document into ~2N chunks, then scrubber B matches N times across them.
// Single_* rows are the control: B alone over an unfragmented document, where
// splices land at the tail and cost nothing.
// Sizes step by 4x so a quadratic term shows as a ~16x step.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class SpliceBenchmarks
{
    const string longToken = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"; // 40 chars
    const string shortToken = "BBBBBBBBBB"; // 10 chars

    static Dictionary<string, object> emptyContext = [];

    string small = null!;
    string medium = null!;
    string large = null!;

    EngineScrubberSet fragmentingSet = null!;
    EngineScrubberSet singleSet = null!;

    [GlobalSetup]
    public void Setup()
    {
        small = Build(200);
        medium = Build(800);
        large = Build(3_200);

        // A has the longer find so the engine orders it first, fragmenting the list
        // before B runs over it
        fragmentingSet = EngineScrubberSet.ForScrubbers(
        [
            Scrubber.Replace(longToken, "{A}"),
            Scrubber.Replace(shortToken, "{B}")
        ]);
        singleSet = EngineScrubberSet.ForScrubbers([Scrubber.Replace(shortToken, "{B}")]);
    }

    // Each line holds one long token and one short token
    static string Build(int lines)
    {
        var builder = new StringBuilder();
        for (var line = 0; line < lines; line++)
        {
            builder.Append("  \"a\": \"");
            builder.Append(longToken);
            builder.Append("\", \"b\": \"");
            builder.Append(shortToken);
            builder.Append("\",\n");
        }

        return builder.ToString();
    }

    static string Run(string input, EngineScrubberSet set)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(input, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    [Benchmark]
    public string Fragmented_200() => Run(small, fragmentingSet);

    [Benchmark]
    public string Fragmented_800() => Run(medium, fragmentingSet);

    [Benchmark]
    public string Fragmented_3200() => Run(large, fragmentingSet);

    [Benchmark]
    public string Single_200() => Run(small, singleSet);

    [Benchmark]
    public string Single_800() => Run(medium, singleSet);

    [Benchmark]
    public string Single_3200() => Run(large, singleSet);
}
