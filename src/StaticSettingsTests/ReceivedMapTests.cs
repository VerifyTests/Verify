// Lives here, rather than in Verify.Tests, since these toggle the build server detection, and this
// project runs serially with BaseTest restoring that state between tests.
public class ReceivedMapTests :
    BaseTest
{
    [Fact]
    public async Task WritesMapToIntermediateDirectory()
    {
        DiffEngine.BuildServerDetector.Detected = false;

        using var temp = new TempDirectory();
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.DisableDiff();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        var received = Received(temp);

        // The map lives in obj, not beside the snapshot, so it neither clutters the snapshot
        // directory nor gets picked up by the `*.received.*` globs used to find snapshots.
        Assert.Empty(Directory.EnumerateFiles(temp, "*.map"));

        Assert.Equal(
            Path.Combine(temp.Path, "ReceivedMapTests.WritesMapToIntermediateDirectory.verified.txt"),
            FindMap(received));
    }

    [Fact]
    public async Task NoMapOnBuildServer()
    {
        DiffEngine.BuildServerDetector.Detected = true;

        using var temp = new TempDirectory();
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.DisableDiff();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        // A received file is still produced, but nothing on a build server consumes a map.
        Assert.NotNull(Received(temp));
        Assert.Null(FindMap(Received(temp)));
    }

    [Fact]
    public async Task NoMapWhenAutoVerified()
    {
        DiffEngine.BuildServerDetector.Detected = false;

        using var temp = new TempDirectory();
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.DisableDiff();
        settings.AutoVerify();

        await Verify("value", settings);

        // The received file is moved to verified, so there is nothing for a map to describe.
        var received = Path.Combine(temp.Path, "ReceivedMapTests.NoMapWhenAutoVerified.received.txt");
        Assert.Null(FindMap(received));
    }

    [Fact]
    public async Task MapIsOverwrittenNotDuplicated()
    {
        DiffEngine.BuildServerDetector.Detected = false;

        using var temp = new TempDirectory();

        VerifySettings Settings()
        {
            var settings = new VerifySettings();
            settings.UseDirectory(temp);
            settings.DisableDiff();
            settings.DisableRequireUniquePrefix();
            return settings;
        }

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings()));
        var first = MapFiles().Count;

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings()));

        // The map name is derived from the received path, so a re run overwrites it rather than
        // accumulating a new file each time.
        Assert.Equal(first, MapFiles().Count);
    }

    [Fact]
    public async Task MapEnablesAccept()
    {
        DiffEngine.BuildServerDetector.Detected = false;

        using var temp = new TempDirectory();

        VerifySettings Settings()
        {
            var settings = new VerifySettings();
            settings.UseDirectory(temp);
            settings.DisableDiff();
            settings.DisableRequireUniquePrefix();
            return settings;
        }

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings()));

        // Accept the way out of process tooling would: read the verified path from the map, rather
        // than trying to derive it from the received name.
        var received = Received(temp);
        File.Move(received, FindMap(received)!, true);

        // The accept landed on the file Verify expects, so the next run passes.
        await Verify("value", Settings());
    }

    static string MapDirectory =>
        Path.Combine(AttributeReader.GetIntermediateDirectory(typeof(ReceivedMapTests).Assembly), "VerifyReceived");

    static List<string> MapFiles() =>
        Directory.Exists(MapDirectory) ? Directory.EnumerateFiles(MapDirectory).ToList() : [];

    // Maps are named from a hash of the received path, so they are located by content rather than name.
    static string? FindMap(string receivedPath)
    {
        foreach (var file in MapFiles())
        {
            var lines = File.ReadAllLines(file);
            if (lines.Length == 2 &&
                lines[0] == receivedPath)
            {
                return lines[1];
            }
        }

        return null;
    }

    static string Received(string directory) =>
        Directory.EnumerateFiles(directory)
            .Single(_ => _.EndsWith(".received.txt", StringComparison.Ordinal));
}
