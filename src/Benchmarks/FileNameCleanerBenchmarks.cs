[MemoryDiagnoser(false)]
public class FileNameCleanerBenchmarks
{
    [Benchmark]
    public string ReplaceInvalidFileNameChars() =>
        "Ant apple | The Bear Fox > Theater".ReplaceInvalidFileNameChars();
}