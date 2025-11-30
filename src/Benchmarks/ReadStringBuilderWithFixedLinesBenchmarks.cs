[MemoryDiagnoser(false)]
public class ReadStringBuilderWithFixedLinesBenchmarks
{
    List<string> files;

    public ReadStringBuilderWithFixedLinesBenchmarks()
    {
        var solutionDirectory = SolutionDirectoryFinder.Find(Environment.CurrentDirectory);
        files = Directory.EnumerateFiles(
                solutionDirectory,
                "*.verified.txt",
                SearchOption.AllDirectories)
            .ToList();
    }

    [Benchmark]
    public async Task ReadStringBuilderWithFixedLines()
    {
        foreach (var file in files)
        {
            await using var reader = IoHelpers.OpenRead(file);
            await reader.ReadStringBuilderWithFixedLines();
        }
    }
}