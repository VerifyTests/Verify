[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class FilterLinesBenchmarks
{
    StringBuilder smallInput = null!;
    StringBuilder mediumInput = null!;
    StringBuilder largeInput = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Small: ~1KB, 20 lines
        smallInput = CreateTestData(20, 50);

        // Medium: ~50KB, 1000 lines
        mediumInput = CreateTestData(1000, 50);

        // Large: ~500KB, 10000 lines
        largeInput = CreateTestData(10000, 50);
    }

    static StringBuilder CreateTestData(int lineCount, int charsPerLine)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < lineCount; i++)
        {
            builder.Append(new string('x', charsPerLine));
            builder.Append(i);
            builder.AppendLine();
        }
        return builder;
    }

    // Remove every other line
    static bool RemoveEvenLines(string line) =>
        line.Length > 0 && char.IsDigit(line[^1]) && (line[^1] - '0') % 2 == 0;

    [Benchmark(Baseline = true)]
    public void Small()
    {
        var builder = new StringBuilder(smallInput.ToString());
        builder.FilterLines(RemoveEvenLines);
    }

    [Benchmark]
    public void Medium()
    {
        var builder = new StringBuilder(mediumInput.ToString());
        builder.FilterLines(RemoveEvenLines);
    }

    [Benchmark]
    public void Large()
    {
        var builder = new StringBuilder(largeInput.ToString());
        builder.FilterLines(RemoveEvenLines);
    }
}