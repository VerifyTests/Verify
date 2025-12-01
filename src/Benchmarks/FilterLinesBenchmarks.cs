[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class FilterLinesBenchmarks
{
    private StringBuilder _smallInput = null!;
    private StringBuilder _mediumInput = null!;
    private StringBuilder _largeInput = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Small: ~1KB, 20 lines
        _smallInput = CreateTestData(20, 50);

        // Medium: ~50KB, 1000 lines
        _mediumInput = CreateTestData(1000, 50);

        // Large: ~500KB, 10000 lines
        _largeInput = CreateTestData(10000, 50);
    }

    private static StringBuilder CreateTestData(int lineCount, int charsPerLine)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < lineCount; i++)
        {
            sb.Append(new string('x', charsPerLine));
            sb.Append(i);
            sb.AppendLine();
        }
        return sb;
    }

    // Remove every other line
    private static bool RemoveEvenLines(ReadOnlySpan<char> line) =>
        line.Length > 0 && char.IsDigit(line[^1]) && (line[^1] - '0') % 2 == 0;

    [Benchmark(Baseline = true)]
    public void Original_Small()
    {
        var sb = new StringBuilder(_smallInput.ToString());
        sb.FilterLines_Original(RemoveEvenLines);
    }

    [Benchmark]
    public void SpanBased_Small()
    {
        var sb = new StringBuilder(_smallInput.ToString());
        sb.FilterLines_SpanBased(RemoveEvenLines);
    }

    [Benchmark]
    public void ArrayPool_Small()
    {
        var sb = new StringBuilder(_smallInput.ToString());
        sb.FilterLines_ArrayPool(RemoveEvenLines);
    }

    [Benchmark]
    public void Original_Medium()
    {
        var sb = new StringBuilder(_mediumInput.ToString());
        sb.FilterLines_Original(RemoveEvenLines);
    }

    [Benchmark]
    public void SpanBased_Medium()
    {
        var sb = new StringBuilder(_mediumInput.ToString());
        sb.FilterLines_SpanBased(RemoveEvenLines);
    }

    [Benchmark]
    public void ArrayPool_Medium()
    {
        var sb = new StringBuilder(_mediumInput.ToString());
        sb.FilterLines_ArrayPool(RemoveEvenLines);
    }

    [Benchmark]
    public void Original_Large()
    {
        var sb = new StringBuilder(_largeInput.ToString());
        sb.FilterLines_Original(RemoveEvenLines);
    }

    [Benchmark]
    public void SpanBased_Large()
    {
        var sb = new StringBuilder(_largeInput.ToString());
        sb.FilterLines_SpanBased(RemoveEvenLines);
    }

    [Benchmark]
    public void ArrayPool_Large()
    {
        var sb = new StringBuilder(_largeInput.ToString());
        sb.FilterLines_ArrayPool(RemoveEvenLines);
    }
}