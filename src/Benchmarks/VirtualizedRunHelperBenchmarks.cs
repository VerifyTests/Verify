[MemoryDiagnoser(false)]
public class VirtualizedRunHelperBenchmarks
{
    static string solutionDirectory;
    static string projectDirectory;
    static string currentDirectory;

    static VirtualizedRunHelperBenchmarks()
    {
        currentDirectory = Environment.CurrentDirectory;
        solutionDirectory = SolutionDirectoryFinder.Find(currentDirectory);
        projectDirectory = Path.Combine(solutionDirectory, "Verify.Tests");
    }

    [Benchmark]
    public string GetMappedBuildPath()
    {
        var helper = new VirtualizedRunHelper(solutionDirectory, projectDirectory);
        return helper.GetMappedBuildPath(currentDirectory);
    }
}