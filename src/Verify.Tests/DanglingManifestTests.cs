#pragma warning disable VerifyDanglingSnapshots
public class DanglingManifestTests
{
    static readonly DateTime noStaleFiltering = DateTime.MinValue;

    [Fact]
    public void SingleFrameworkIsCoveredByItsOwnManifest()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["one.verified.txt", "two.verified.txt"]);

        var (tracked, covered) = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.True(covered);
        Assert.Equal(["one.verified.txt", "two.verified.txt"], tracked.Order());
    }

    /// <summary>
    /// The point of the manifests: the run of one framework sees what the runs of the others
    /// tracked, so their files are not mistaken for dangling ones.
    /// </summary>
    [Fact]
    public void EveryFrameworkPresentIsCovered()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net8.0", ["Alive.DotNet8_0.verified.txt"]);
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"]);

        var (tracked, covered) = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.True(covered);
        Assert.Equal(["Alive.DotNet8_0.verified.txt", "Alive.DotNet9_0.verified.txt"], tracked.Order());
    }

    /// <summary>
    /// The first framework to run has only its own manifest, so it cannot account for the files of
    /// the frameworks still to run and has to fall back.
    /// </summary>
    [Fact]
    public void MissingFrameworkIsNotCovered()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"]);

        var (tracked, covered) = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.False(covered);
        Assert.Equal(["Alive.DotNet9_0.verified.txt"], tracked);
    }

    /// <summary>
    /// A manifest naming a framework the project no longer targets is read for its paths, but says
    /// nothing about coverage.
    /// </summary>
    [Fact]
    public void RetiredFrameworkDoesNotSatisfyCoverage()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net7.0", ["Alive.DotNet7_0.verified.txt"]);
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"]);

        var (_, covered) = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.False(covered);
    }

    /// <summary>
    /// A manifest from before the last build may name files that have since been renamed or
    /// deleted, and counting those as tracked would hide exactly the danglers the check is for.
    /// </summary>
    [Fact]
    public void StaleManifestIsIgnored()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net8.0", ["Stale.DotNet8_0.verified.txt"]);
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"]);

        // Pinned rather than left to the clock: the two manifests are written milliseconds apart,
        // and file timestamp granularity is coarser than that on some file systems.
        var buildTime = DateTime.UtcNow;
        File.SetLastWriteTimeUtc(
            DanglingManifest.PathFor(directory, "net8.0"),
            buildTime.AddHours(-1));
        File.SetLastWriteTimeUtc(
            DanglingManifest.PathFor(directory, "net9.0"),
            buildTime.AddHours(1));

        var (tracked, covered) = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", buildTime);

        Assert.False(covered);
        Assert.Equal(["Alive.DotNet9_0.verified.txt"], tracked);
    }

    /// <summary>
    /// A re run of one framework replaces its manifest rather than adding to it, so a file that run
    /// no longer tracks stops being tracked.
    /// </summary>
    [Fact]
    public void ReRunReplacesManifest()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Before.verified.txt"]);
        DanglingManifest.Write(directory, "net9.0", ["After.verified.txt"]);

        var (tracked, _) = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.Equal(["After.verified.txt"], tracked);
    }

    /// <summary>
    /// Several verifications can resolve to one verified path, so the bag feeding the manifest can
    /// hold duplicates.
    /// </summary>
    [Fact]
    public void DuplicatesAreWrittenOnce()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Alive.verified.txt", "Alive.verified.txt"]);

        var lines = File.ReadAllLines(DanglingManifest.PathFor(directory, "net9.0"));

        Assert.Equal(["Alive.verified.txt"], lines);
    }

    [Fact]
    public void NoDirectoryIsNotCovered()
    {
        using var directory = new TempDirectory();
        var missing = directory.BuildPath("absent");

        var (tracked, covered) = DanglingManifest.Read(missing, ["net9.0"], "net9.0", noStaleFiltering);

        Assert.False(covered);
        Assert.Empty(tracked);
    }

    [Fact]
    public void EmptyManifestIsStillCoverage()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", []);

        var (tracked, covered) = DanglingManifest.Read(directory, ["net9.0"], "net9.0", noStaleFiltering);

        Assert.True(covered);
        Assert.Empty(tracked);
    }

    /// <summary>
    /// A verified path can be anything the file system accepts, so the manifest has to survive
    /// spaces and the characters Verify puts in a name for parameters, targets and indexes.
    /// </summary>
    [Fact]
    public void AwkwardPathsRoundTrip()
    {
        using var directory = new TempDirectory();
        string[] paths =
        [
            @"D:\a b\Tests.Method_param=a b.DotNet9_0#00.verified.txt",
            "/home/a b/Tests.Method#name.verified.txt",
            @"D:\a\Tests.Method_param=Ünïcödé.verified.txt"
        ];
        DanglingManifest.Write(directory, "net9.0", paths);

        var (tracked, _) = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.Equal(paths.Order(), tracked.Order());
    }

    /// <summary>
    /// Written through a temporary file and moved into place. A partially written manifest read by
    /// another framework's run would drop paths and report live snapshots as dangling.
    /// </summary>
    [Fact]
    public void WriteLeavesNoTemporaryFiles()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Alive.verified.txt"]);

        var files = Directory.GetFiles(directory)
            .Select(Path.GetFileName)
            .Order();

        Assert.Equal(["net9.0.txt"], files);
    }

    /// <summary>
    /// End to end: two frameworks, one snapshot each, and one left behind by a deleted test. Only
    /// the union can tell them apart, and only when it is complete.
    /// </summary>
    [Fact]
    public Task OrphanSurvivesTheUnion()
    {
        using var directory = new TempDirectory();
        var root = directory.Path;
        var aliveOnNet8 = Path.Combine(root, "Tests.Alive.DotNet8_0.verified.txt");
        var aliveOnNet9 = Path.Combine(root, "Tests.Alive.DotNet9_0.verified.txt");
        var orphan = Path.Combine(root, "Tests.Deleted.DotNet8_0.verified.txt");

        DanglingManifest.Write(directory, "net8.0", [aliveOnNet8]);
        DanglingManifest.Write(directory, "net9.0", [aliveOnNet9]);

        var (tracked, covered) = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.True(covered);
        return Throws(
                () => DanglingSnapshotsCheck.CheckFiles(
                    [aliveOnNet8, aliveOnNet9, orphan],
                    tracked,
                    root,
                    covered))
            .IgnoreStackTrace();
    }

    /// <summary>
    /// The same inputs without full coverage. The orphan carries a framework segment, so the
    /// fallback skips it and reports nothing.
    /// </summary>
    [Fact]
    public void OrphanIsInvisibleWithoutCoverage()
    {
        using var directory = new TempDirectory();
        var root = directory.Path;
        var aliveOnNet9 = Path.Combine(root, "Tests.Alive.DotNet9_0.verified.txt");
        var orphan = Path.Combine(root, "Tests.Deleted.DotNet8_0.verified.txt");

        DanglingManifest.Write(directory, "net9.0", [aliveOnNet9]);

        var (tracked, covered) = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.False(covered);
        DanglingSnapshotsCheck.CheckFiles([aliveOnNet9, orphan], tracked, root, covered);
    }
}
