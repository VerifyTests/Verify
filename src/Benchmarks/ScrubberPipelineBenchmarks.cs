using VerifyTests;

[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class ScrubberPipelineBenchmarks
{
    string tinyInput = null!;
    string mediumInput = null!;
    string largeInput = null!;
    string noMatchInput = null!;

    VerifySettings settings = null!;
    Counter counter = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Tiny: a single Guid in a short line.
        tinyInput = "Id: 173535ae-995b-4cc6-a74e-8cd4be57039c";

        // Medium: ~2 KB, 10 log lines mixing guids, ISO datetimes, paths.
        mediumInput = string.Join('\n', Enumerable.Range(0, 10).Select(BuildLogLine));

        // Large: ~200 KB, repeat medium 100 times.
        largeInput = string.Join('\n', Enumerable.Repeat(mediumInput, 100));

        // NoMatch: ~200 KB of plain text containing nothing the active scrubbers care about.
        // Worst case for length-window filtering / pure throughput.
        noMatchInput = string.Concat(Enumerable.Repeat("the quick brown fox jumps over the lazy dog\n", 5000));

        settings = new VerifySettings();
        counter = Counter.Start();
    }

    static string BuildLogLine(int i) =>
        $"[2025-01-{i:D2}T10:11:12.{i:D3}Z] request {Guid.NewGuid():D} from /usr/local/var/lib/thing-{i}.log finished";

    StringBuilder BuildBuilder(string input) => new(input);

    // ---- New pipeline ----

    [Benchmark]
    public void New_Tiny_Guid()
    {
        var builder = BuildBuilder(tinyInput);
        ScrubberPipeline_PublicRunner.RunWithGuid(builder, counter);
    }

    [Benchmark]
    public void New_Medium_Mixed()
    {
        var builder = BuildBuilder(mediumInput);
        ScrubberPipeline_PublicRunner.RunWithMixed(builder, counter);
    }

    [Benchmark]
    public void New_Large_Mixed()
    {
        var builder = BuildBuilder(largeInput);
        ScrubberPipeline_PublicRunner.RunWithMixed(builder, counter);
    }

    [Benchmark]
    public void New_NoMatch()
    {
        var builder = BuildBuilder(noMatchInput);
        ScrubberPipeline_PublicRunner.RunWithMixed(builder, counter);
    }

    // ---- Old pipeline (legacy delegate-based scrubber via PatternScrubberRunner) ----

    [Benchmark(Baseline = true)]
    public void Old_Tiny_Guid()
    {
        var builder = BuildBuilder(tinyInput);
        GuidScrubber.ReplaceGuids(builder, counter);
    }

    [Benchmark]
    public void Old_Medium_Mixed()
    {
        var builder = BuildBuilder(mediumInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public void Old_Large_Mixed()
    {
        var builder = BuildBuilder(largeInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public void Old_NoMatch()
    {
        var builder = BuildBuilder(noMatchInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }
}

// Helper that drives the new public pipeline through the same set of registered scrubbers each iteration.
static class ScrubberPipeline_PublicRunner
{
    static readonly DateTimeOffsetPatternScrubber dateTimeOffset =
        new("yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture);

    public static void RunWithGuid(StringBuilder builder, Counter counter)
    {
        var settings = new VerifySettings();
        settings.AddScrubber(GuidPatternScrubber.Instance);
        ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
    }

    public static void RunWithMixed(StringBuilder builder, Counter counter)
    {
        var settings = new VerifySettings();
        settings.AddScrubber(GuidPatternScrubber.Instance);
        settings.AddScrubber(dateTimeOffset);
        ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
    }
}
