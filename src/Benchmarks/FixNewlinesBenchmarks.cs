// Sizes and newline distribution are calibrated from a scan of 13,471 *.verified.* text files
// across C:\Code (2026-05): p50=608B, p90=5.8KB, p99=43KB, ~84 bytes/line, 0.4% contain CR.
// LF-only exercises the Contains('\r') short-circuit; CrLf exercises the full Replace path.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class FixNewlinesBenchmarks
{
    string smallLf = null!;
    string mediumLf = null!;
    string largeLf = null!;
    string smallCrLf = null!;
    string mediumCrLf = null!;
    string largeCrLf = null!;

    [GlobalSetup]
    public void Setup()
    {
        smallLf = CreateContent(7, "\n");          // ~600 B  (p50)
        mediumLf = CreateContent(60, "\n");        // ~5 KB   (p90)
        largeLf = CreateContent(475, "\n");        // ~40 KB  (p99)
        smallCrLf = CreateContent(7, "\r\n");
        mediumCrLf = CreateContent(60, "\r\n");
        largeCrLf = CreateContent(475, "\r\n");
    }

    static string CreateContent(int lineCount, string newline)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < lineCount; i++)
        {
            builder.Append('x', 80);
            builder.Append(i);
            builder.Append(newline);
        }
        return builder.ToString();
    }

    static void Run(string contents)
    {
        var builder = new StringBuilder(contents);
        if (contents.Contains('\r'))
        {
            builder.FixNewlines();
        }
    }

    [Benchmark(Baseline = true)]
    public void Small_LfOnly() => Run(smallLf);

    [Benchmark]
    public void Medium_LfOnly() => Run(mediumLf);

    [Benchmark]
    public void Large_LfOnly() => Run(largeLf);

    [Benchmark]
    public void Small_CrLf() => Run(smallCrLf);

    [Benchmark]
    public void Medium_CrLf() => Run(mediumCrLf);

    [Benchmark]
    public void Large_CrLf() => Run(largeCrLf);
}
