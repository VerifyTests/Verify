// TempDirectory and TempFile each held a verbatim copy of the tracked-path matcher.
// Duplicated_* rows run that copy, built inline so it closes over statics only.
// Shared_* rows run the extracted TempPathScrubber.Build, whose matcher closes over
// its parameters instead. The question is whether that extra closure indirection
// costs anything on the scrub path.
// Document sizes follow the *.verified.* scan used by the other scrubber benchmarks:
// p50=260 chars, p90=2.9KB, p99=31KB. NoPaths is the dominant real case, since the
// async local is null unless a test actually created a temp path.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class TempPathScrubberBenchmarks
{
    static AsyncLocal<List<string>?> asyncPaths = new();
    static readonly object pathsLock = new();

    // A realistic root, matching [System.Temp]\VerifyTempDirectory
    static string rootDirectory = Path.Combine(Path.GetTempPath(), "VerifyTempDirectory");
    static string trackedPath = Path.Combine(rootDirectory, "abcd1234.xyz");

    static Dictionary<string, object> emptyContext = [];

    string small = null!;
    string medium = null!;
    string large = null!;
    string largeWithPaths = null!;

    EngineScrubberSet duplicatedSet = null!;
    EngineScrubberSet sharedSet = null!;

    [GlobalSetup]
    public void Setup()
    {
        small = Build(260, false);
        medium = Build(2_900, false);
        large = Build(31_000, false);
        largeWithPaths = Build(31_000, true);

        duplicatedSet = EngineScrubberSet.ForScrubbers([BuildDuplicated()]);
        sharedSet = EngineScrubberSet.ForScrubbers([TempPathScrubber.Build(asyncPaths, pathsLock, "{TempDirectory}", rootDirectory.Length)]);
    }

    // Lines of ~40 chars; when withPaths, every 12th line embeds the tracked path
    static string Build(int targetChars, bool withPaths)
    {
        var builder = new StringBuilder();
        var line = 0;
        while (builder.Length < targetChars)
        {
            if (withPaths &&
                line % 12 == 0)
            {
                builder.Append("  \"path\": \"");
                builder.Append(trackedPath);
                builder.Append("\",");
            }
            else
            {
                builder.Append("  \"name\": \"some value here\",");
            }

            builder.Append('\n');
            line++;
        }

        return builder.ToString();
    }

    // The matcher as it was duplicated in TempDirectory.Init and TempFile.Init
    static Scrubber BuildDuplicated() =>
        Scrubber.Match(
            (ReadOnlySpan<char> segment, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
            {
                index = -1;
                length = 0;
                replacement = "{TempDirectory}";
                var pathsValue = asyncPaths.Value;
                if (pathsValue == null)
                {
                    return false;
                }

                lock (pathsLock)
                {
                    foreach (var path in pathsValue)
                    {
                        var found = segment.IndexOf(path.AsSpan(), StringComparison.Ordinal);
                        if (found < 0)
                        {
                            continue;
                        }

                        if (index < 0 ||
                            found < index)
                        {
                            index = found;
                            length = path.Length;
                        }
                    }
                }

                return index >= 0;
            },
            minLength: rootDirectory.Length);

    static string Run(string input, EngineScrubberSet set)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(input, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    static string RunWithPaths(string input, EngineScrubberSet set)
    {
        asyncPaths.Value = [trackedPath];
        try
        {
            return Run(input, set);
        }
        finally
        {
            asyncPaths.Value = null;
        }
    }

    [Benchmark]
    public string Duplicated_NoPaths_Small() => Run(small, duplicatedSet);

    [Benchmark]
    public string Shared_NoPaths_Small() => Run(small, sharedSet);

    [Benchmark]
    public string Duplicated_NoPaths_Medium() => Run(medium, duplicatedSet);

    [Benchmark]
    public string Shared_NoPaths_Medium() => Run(medium, sharedSet);

    [Benchmark]
    public string Duplicated_NoPaths_Large() => Run(large, duplicatedSet);

    [Benchmark]
    public string Shared_NoPaths_Large() => Run(large, sharedSet);

    [Benchmark]
    public string Duplicated_PathsNoMatch_Large() => RunWithPaths(large, duplicatedSet);

    [Benchmark]
    public string Shared_PathsNoMatch_Large() => RunWithPaths(large, sharedSet);

    [Benchmark]
    public string Duplicated_PathsMatch_Large() => RunWithPaths(largeWithPaths, duplicatedSet);

    [Benchmark]
    public string Shared_PathsMatch_Large() => RunWithPaths(largeWithPaths, sharedSet);
}
