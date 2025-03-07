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
        var directory = "path/to";

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, DanglingSnapshotsCheck.OnFailure.Throw, directory))
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
        var directory = "path/to";

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, DanglingSnapshotsCheck.OnFailure.Throw, directory))
            .IgnoreStackTrace();
    }

    [Fact]
    public void AllTracked()
    {
        var filesOnDisk = new List<string> { "path/to/tracked.verified.txt" };
        var trackedFiles = new ConcurrentBag<string> { "path/to/tracked.verified.txt" };
        var directory = "path/to";

        DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, DanglingSnapshotsCheck.OnFailure.Throw, directory);
    }
}