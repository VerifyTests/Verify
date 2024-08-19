[MemoryDiagnoser(false)]
public class ReadStringBuilderWithFixedLinesBenchmarks
{
    readonly IEnumerable<string> files;

    public ReadStringBuilderWithFixedLinesBenchmarks()
    {
        var solutionDirectory = SolutionDirectoryFinder.Find(Environment.CurrentDirectory);;
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
            using var reader = IoHelpers.OpenRead(file);
            await IoHelpers.ReadStringBuilderWithFixedLines(reader);
        }
    }
}