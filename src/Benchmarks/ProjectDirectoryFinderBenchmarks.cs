[MemoryDiagnoser(false)]
public class ProjectDirectoryFinderBenchmarks
{
    static string directory;

    static ProjectDirectoryFinderBenchmarks()
    {
        var solutionDirectory = SolutionDirectoryFinder.Find(Environment.CurrentDirectory);
        directory = Path.Combine(solutionDirectory, @"Verify.Tests\VerifyDirectoryTests.WithDirectory\nested.with.dot");
    }

    [Benchmark]
    public string? FindProjectDirectory()
    {
        ProjectDirectoryFinder.TryFind(directory, out var path);
        return path;
    }
}