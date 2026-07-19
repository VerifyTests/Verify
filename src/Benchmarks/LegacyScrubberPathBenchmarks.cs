// When any legacy AddScrubber(Action<StringBuilder>) is registered, the span engine
// runs first and a StringBuilder pass follows. The trailing path replacement and
// newline fix then run over that builder.
// PropertyValue_* is the hot one: it runs once per serialized string value, and it
// materialized the builder twice, once inside the path pass and once to return.
// Document_* runs once per file and materializes once.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class LegacyScrubberPathBenchmarks
{
    string small = null!;
    string medium = null!;
    string large = null!;

    VerifySettings settings = null!;
    Counter counter = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Deterministic path replacements, so the trailing pass has real work to scan
        DirectoryReplacements.UseAssembly("C:/Code/TheSolution", "C:/Code/TheSolution/TheProject");

        small = Build(260);
        medium = Build(2_900);
        large = Build(31_000);

        settings = new();
        // A no-op legacy scrubber, enough to select the legacy pipeline
        settings.AddScrubber(_ => { });

        counter = Counter.Start();
    }

    [GlobalCleanup]
    public void Cleanup() => counter.Dispose();

    static string Build(int targetChars)
    {
        var builder = new StringBuilder();
        while (builder.Length < targetChars)
        {
            builder.Append("  \"name\": \"some value here\",\n");
        }

        return builder.ToString();
    }

    [Benchmark]
    public string PropertyValue_Small() => ApplyScrubbers.ApplyForPropertyValue(small, settings, counter);

    [Benchmark]
    public string PropertyValue_Medium() => ApplyScrubbers.ApplyForPropertyValue(medium, settings, counter);

    [Benchmark]
    public string PropertyValue_Large() => ApplyScrubbers.ApplyForPropertyValue(large, settings, counter);

    [Benchmark]
    public int Document_Large()
    {
        var builder = new StringBuilder(large);
        ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
        return builder.Length;
    }
}
