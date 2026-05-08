using VerifyTests;

[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class ScrubberPipelineBenchmarks
{
    string tinyInput = null!;
    string mediumInput = null!;
    string largeInput = null!;
    string noMatchInput = null!;
    string shortLinesInput = null!;

    VerifySettings settingsGuid = null!;
    VerifySettings settingsMixed = null!;
    Counter counter = null!;
    DateTimeOffsetPatternScrubber dateTimeOffset = null!;

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
        noMatchInput = string.Concat(Enumerable.Repeat("the quick brown fox jumps over the lazy dog\n", 5000));

        // ShortLines: ~200 KB where every line is shorter than the Guid scrubber MinLength (36 chars).
        // Exercises the per-line skip-by-length-window optimization: the new pipeline can decide
        // each line is too short to match without invoking TryMatch.
        shortLinesInput = string.Concat(Enumerable.Repeat("hello world\n", 18000));

        dateTimeOffset = new("yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture);

        // Pre-built settings reused across iterations so the benchmark measures
        // the engine, not the registration cost.
        settingsGuid = new VerifySettings();
        settingsGuid.AddScrubber(GuidPatternScrubber.Instance);

        settingsMixed = new VerifySettings();
        settingsMixed.AddScrubber(GuidPatternScrubber.Instance);
        settingsMixed.AddScrubber(dateTimeOffset);

        counter = Counter.Start();
    }

    static string BuildLogLine(int i) =>
        $"[2025-01-{i:D2}T10:11:12.{i:D3}Z] request {Guid.NewGuid():D} from /usr/local/var/lib/thing-{i}.log finished";

    // ---- New pipeline ----

    [Benchmark]
    public void New_Tiny_Guid()
    {
        var builder = new StringBuilder(tinyInput);
        ApplyScrubbers.ApplyForExtension("txt", builder, settingsGuid, counter);
    }

    [Benchmark]
    public void New_Medium_Mixed()
    {
        var builder = new StringBuilder(mediumInput);
        ApplyScrubbers.ApplyForExtension("txt", builder, settingsMixed, counter);
    }

    [Benchmark]
    public void New_Large_Mixed()
    {
        var builder = new StringBuilder(largeInput);
        ApplyScrubbers.ApplyForExtension("txt", builder, settingsMixed, counter);
    }

    [Benchmark]
    public void New_NoMatch()
    {
        var builder = new StringBuilder(noMatchInput);
        ApplyScrubbers.ApplyForExtension("txt", builder, settingsMixed, counter);
    }

    [Benchmark]
    public void New_ShortLines()
    {
        var builder = new StringBuilder(shortLinesInput);
        ApplyScrubbers.ApplyForExtension("txt", builder, settingsMixed, counter);
    }

    // ---- Old pipeline (legacy delegate-based scrubber) ----

    [Benchmark(Baseline = true)]
    public void Old_Tiny_Guid()
    {
        var builder = new StringBuilder(tinyInput);
        GuidScrubber.ReplaceGuids(builder, counter);
    }

    [Benchmark]
    public void Old_Medium_Mixed()
    {
        var builder = new StringBuilder(mediumInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public void Old_Large_Mixed()
    {
        var builder = new StringBuilder(largeInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public void Old_NoMatch()
    {
        var builder = new StringBuilder(noMatchInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public void Old_ShortLines()
    {
        var builder = new StringBuilder(shortLinesInput);
        GuidScrubber.ReplaceGuids(builder, counter);
        DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", counter, CultureInfo.InvariantCulture);
    }
}
