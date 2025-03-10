public class DanglingSnapshotsCheckTests
{
    [Fact]
    public Task Untracked()
    {
        var filesOnDisk = new List<string>
        {
            "path/to/untracked.verified.txt"
        };
        var trackedFiles = new ConcurrentBag<string>
        {
            "path/to/tracked.verified.txt"
        };

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, "path/to"))
            .IgnoreStackTrace();
    }

    [Fact]
    public Task IncorrectCase()
    {
        var filesOnDisk = new List<string>
        {
            "path/to/Tracked.verified.txt"
        };
        var trackedFiles = new ConcurrentBag<string>
        {
            "path/to/tracked.verified.txt"
        };

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, "path/to"))
            .IgnoreStackTrace();
    }

    [Fact]
    public void AllTracked()
    {
        var filesOnDisk = new List<string> { "path/to/tracked.verified.txt" };
        var trackedFiles = new ConcurrentBag<string> { "path/to/tracked.verified.txt" };

        DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, "path/to");
    }
}