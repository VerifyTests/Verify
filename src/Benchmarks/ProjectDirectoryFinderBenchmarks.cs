[MemoryDiagnoser(false)]
public class ProjectDirectoryFinderBenchmarks
{
    static string directory = Path.Combine(
        AttributeReader.GetSolutionDirectory(),
        @"Verify.Tests\VerifyDirectoryTests.WithDirectory");

    [Benchmark]
    public string? ScrubStackTrace()
    {
        ProjectDirectoryFinder.TryFind(directory, out var path);
        return path;
    }
}