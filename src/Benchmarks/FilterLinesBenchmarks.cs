// Predicate based line removal (ScrubLines). Legacy_* rows run the pre-engine
// StringReader implementation; Engine_* rows run the span engine line phase with a
// string predicate (the engine materializes each line lazily for the predicate).
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class FilterLinesBenchmarks
{
    string smallInput = null!;
    string mediumInput = null!;
    string largeInput = null!;

    EngineScrubberSet removeEvenSet = null!;
    EngineScrubberSet neverMatchesSet = null!;
    EngineScrubberSet spanRemoveEvenSet = null!;
    EngineScrubberSet spanNeverMatchesSet = null!;
    static Dictionary<string, object> emptyContext = [];

    [GlobalSetup]
    public void Setup()
    {
        // Small: ~1KB, 20 lines
        smallInput = CreateTestData(20, 50);

        // Medium: ~50KB, 1000 lines
        mediumInput = CreateTestData(1000, 50);

        // Large: ~500KB, 10000 lines
        largeInput = CreateTestData(10000, 50);

        removeEvenSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLines(RemoveEvenLines)]);
        neverMatchesSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLines(NeverMatches)]);
        spanRemoveEvenSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLines((LineMatch) RemoveEvenLinesSpan)]);
        spanNeverMatchesSet = EngineScrubberSet.ForScrubbers([Scrubber.RemoveLines((LineMatch) NeverMatchesSpan)]);
    }

    static string CreateTestData(int lineCount, int charsPerLine)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < lineCount; i++)
        {
            builder.Append('x', charsPerLine);
            builder.Append(i);
            builder.AppendLine();
        }

        return builder.ToString();
    }

    // Remove every other line
    static bool RemoveEvenLines(string line) =>
        line.Length > 0 && char.IsDigit(line[^1]) && (line[^1] - '0') % 2 == 0;

    // Never matches - no lines removed
    static bool NeverMatches(string line) => false;

    // Span variants: no per-line string is materialized for these
    static bool RemoveEvenLinesSpan(ReadOnlySpan<char> line) =>
        line.Length > 0 && char.IsDigit(line[^1]) && (line[^1] - '0') % 2 == 0;

    static bool NeverMatchesSpan(ReadOnlySpan<char> line) => false;

    string Engine(EngineScrubberSet set, string content)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(content, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    [Benchmark(Baseline = true)]
    public void Legacy_Small()
    {
        var builder = new StringBuilder(smallInput);
        builder.FilterLines(RemoveEvenLines);
    }

    [Benchmark]
    public void Legacy_Medium()
    {
        var builder = new StringBuilder(mediumInput);
        builder.FilterLines(RemoveEvenLines);
    }

    [Benchmark]
    public void Legacy_Large()
    {
        var builder = new StringBuilder(largeInput);
        builder.FilterLines(RemoveEvenLines);
    }

    [Benchmark]
    public void Legacy_Small_NoMatches()
    {
        var builder = new StringBuilder(smallInput);
        builder.FilterLines(NeverMatches);
    }

    [Benchmark]
    public void Legacy_Medium_NoMatches()
    {
        var builder = new StringBuilder(mediumInput);
        builder.FilterLines(NeverMatches);
    }

    [Benchmark]
    public void Legacy_Large_NoMatches()
    {
        var builder = new StringBuilder(largeInput);
        builder.FilterLines(NeverMatches);
    }

    [Benchmark]
    public string Engine_Small() => Engine(removeEvenSet, smallInput);

    [Benchmark]
    public string Engine_Medium() => Engine(removeEvenSet, mediumInput);

    [Benchmark]
    public string Engine_Large() => Engine(removeEvenSet, largeInput);

    [Benchmark]
    public string Engine_Small_NoMatches() => Engine(neverMatchesSet, smallInput);

    [Benchmark]
    public string Engine_Medium_NoMatches() => Engine(neverMatchesSet, mediumInput);

    [Benchmark]
    public string Engine_Large_NoMatches() => Engine(neverMatchesSet, largeInput);

    [Benchmark]
    public string EngineSpan_Small() => Engine(spanRemoveEvenSet, smallInput);

    [Benchmark]
    public string EngineSpan_Medium() => Engine(spanRemoveEvenSet, mediumInput);

    [Benchmark]
    public string EngineSpan_Large() => Engine(spanRemoveEvenSet, largeInput);

    [Benchmark]
    public string EngineSpan_Small_NoMatches() => Engine(spanNeverMatchesSet, smallInput);

    [Benchmark]
    public string EngineSpan_Medium_NoMatches() => Engine(spanNeverMatchesSet, mediumInput);

    [Benchmark]
    public string EngineSpan_Large_NoMatches() => Engine(spanNeverMatchesSet, largeInput);
}
