public class FileNameCleanerBenchmarksTests
{
    [Fact]
    public Task ReplaceInvalidFileNameChars() =>
        Verify("Ant apple | The Bear Fox > Theater".ReplaceInvalidFileNameChars());
}