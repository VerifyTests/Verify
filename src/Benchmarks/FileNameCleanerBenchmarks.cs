[MemoryDiagnoser(false)]
public class FileNameCleanerBenchmarks
{
    [Benchmark]
    public void ReplaceInvalidFileNameChars()
    {
        "Ant apple | The Bear Fox > Theater".ReplaceInvalidFileNameChars();
        "Ant apple The Bear Fox Theater".ReplaceInvalidFileNameChars();
    }
}