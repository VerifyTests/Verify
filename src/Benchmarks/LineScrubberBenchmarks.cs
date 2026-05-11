// Uses the Func<string, bool> / Func<string, string?> public API style so the
// same source compiles on both `main` and this branch — keep the lambdas portable.
#pragma warning disable CS0618 // obsolete on this branch only
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class LineScrubberBenchmarks
{
    static string mediumInput = null!;
    static string largeInput = null!;

    static VerifySettings filterDropHalf = null!;
    static VerifySettings filterKeepAll = null!;
    static VerifySettings replacePassthrough = null!;
    static VerifySettings replaceModify = null!;
    static VerifySettings removeEmpty = null!;
    static VerifySettings containsAny = null!;

    static VerifySettings filterDropHalfNew = null!;
    static VerifySettings filterKeepAllNew = null!;
    static VerifySettings replacePassthroughNew = null!;
    static VerifySettings replaceModifyNew = null!;

    static Counter counter = null!;

    [GlobalSetup]
    public void Setup()
    {
        mediumInput = string.Join('\n', Enumerable.Range(0, 1000).Select(i => $"line {i:D4} content padding xxxxxxxxxxxxxxxxxxxxxx"));
        largeInput = string.Join('\n', Enumerable.Range(0, 10000).Select(i => $"line {i:D5} content padding xxxxxxxxxxxxxxxxxxxxxx"));

        // Legacy Func<string, ...> API (works on main + this branch via obsolete adapter).
        filterDropHalf = new();
        filterDropHalf.ScrubLines(line => line.Length > 10 && line[^1] == 'x');

        filterKeepAll = new();
        filterKeepAll.ScrubLines(_ => false);

        replacePassthrough = new();
        replacePassthrough.ScrubLinesWithReplace(line => line);

        replaceModify = new();
        replaceModify.ScrubLinesWithReplace(line => line.Contains("padding") ? "REPLACED" : line);

        removeEmpty = new();
        removeEmpty.ScrubEmptyLines();

        containsAny = new();
        containsAny.ScrubLinesContaining(StringComparison.Ordinal, "0042", "0099", "1234");

        // New span-based API (this branch only).
        filterDropHalfNew = new();
        filterDropHalfNew.ScrubLines(line => line.Length > 10 && line[^1] == 'x');

        filterKeepAllNew = new();
        filterKeepAllNew.ScrubLines(_ => false);

        replacePassthroughNew = new();
        replacePassthroughNew.ScrubLinesWithReplace((line, out result) =>
        {
            result = line;
            return true;
        });

        replaceModifyNew = new();
        replaceModifyNew.ScrubLinesWithReplace((line, out result) =>
        {
            if (line.Contains("padding", StringComparison.Ordinal))
            {
                result = "REPLACED";
            }
            else
            {
                result = line;
            }

            return true;
        });

        counter = Counter.Start();
    }

    // ---- Legacy Func<string, bool> API ----

    [Benchmark]
    public void Filter_DropHalf_Medium() => Run(mediumInput, filterDropHalf);

    [Benchmark]
    public void Filter_DropHalf_Large() => Run(largeInput, filterDropHalf);

    [Benchmark]
    public void Filter_KeepAll_Medium() => Run(mediumInput, filterKeepAll);

    [Benchmark]
    public void Filter_KeepAll_Large() => Run(largeInput, filterKeepAll);

    [Benchmark]
    public void Replace_Passthrough_Medium() => Run(mediumInput, replacePassthrough);

    [Benchmark]
    public void Replace_Passthrough_Large() => Run(largeInput, replacePassthrough);

    [Benchmark]
    public void Replace_Modify_Medium() => Run(mediumInput, replaceModify);

    [Benchmark]
    public void RemoveEmpty_Medium() => Run(mediumInput, removeEmpty);

    [Benchmark]
    public void ContainsAny_Medium() => Run(mediumInput, containsAny);

    // ---- New span-based API (this branch only) ----

    [Benchmark]
    public void New_Filter_DropHalf_Medium() => Run(mediumInput, filterDropHalfNew);

    [Benchmark]
    public void New_Filter_DropHalf_Large() => Run(largeInput, filterDropHalfNew);

    [Benchmark]
    public void New_Filter_KeepAll_Medium() => Run(mediumInput, filterKeepAllNew);

    [Benchmark]
    public void New_Filter_KeepAll_Large() => Run(largeInput, filterKeepAllNew);

    [Benchmark]
    public void New_Replace_Passthrough_Medium() => Run(mediumInput, replacePassthroughNew);

    [Benchmark]
    public void New_Replace_Passthrough_Large() => Run(largeInput, replacePassthroughNew);

    [Benchmark]
    public void New_Replace_Modify_Medium() => Run(mediumInput, replaceModifyNew);

    static void Run(string input, VerifySettings settings)
    {
        var builder = new StringBuilder(input);
        ScrubberPipeline.ApplyForExtension("txt", builder, settings, counter);
    }
}
