#pragma warning disable VerifyDanglingSnapshots
public class DanglingManifestTests
{
    static readonly DateTime noStaleFiltering = DateTime.MinValue;

    [Fact]
    public void SingleFrameworkIsCoveredByItsOwnManifest()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["one.verified.txt", "two.verified.txt"], ["one", "two"]);

        var merged = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.True(merged.FrameworksCovered);
        Assert.Equal(["one.verified.txt", "two.verified.txt"], merged.Files.Order());
        Assert.Equal(["one", "two"], merged.Prefixes.Order());
    }

    /// <summary>
    /// The point of the manifests: the run of one framework sees what the runs of the others
    /// tracked, so their files are not mistaken for dangling ones.
    /// </summary>
    [Fact]
    public void EveryFrameworkPresentIsCovered()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net8.0", ["Alive.DotNet8_0.verified.txt"], ["Alive"]);
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"], ["Alive"]);

        var merged = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.True(merged.FrameworksCovered);
        Assert.Equal(["Alive.DotNet8_0.verified.txt", "Alive.DotNet9_0.verified.txt"], merged.Files.Order());
        Assert.Equal(["Alive"], merged.Prefixes);
    }

    /// <summary>
    /// A test behind a <c>#if</c> exists only in the runs of the frameworks that compile it. Without
    /// its prefix in the union, every other framework's run would see its snapshots as belonging to
    /// no test at all, and report them.
    /// </summary>
    [Fact]
    public void FrameworkSpecificTestIsKnownToTheOtherFrameworks()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net48", ["OnlyOnNet48.verified.txt"], ["OnlyOnNet48"]);
        DanglingManifest.Write(directory, "net9.0", ["Shared.verified.txt"], ["Shared"]);

        var merged = DanglingManifest.Read(directory, ["net48", "net9.0"], "net9.0", noStaleFiltering);

        Assert.True(merged.FrameworksCovered);
        Assert.Contains("OnlyOnNet48", merged.Prefixes);
        Assert.Contains("OnlyOnNet48.verified.txt", merged.Files);
    }

    /// <summary>
    /// The first framework to run has only its own manifest, so it cannot account for the files of
    /// the frameworks still to run and has to leave the framework axis to the name.
    /// </summary>
    [Fact]
    public void MissingFrameworkIsNotCovered()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"], ["Alive"]);

        var merged = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.False(merged.FrameworksCovered);
        Assert.Equal(["Alive.DotNet9_0.verified.txt"], merged.Files);
    }

    /// <summary>
    /// A manifest naming a framework the project no longer targets is read for its contents, but
    /// says nothing about coverage.
    /// </summary>
    [Fact]
    public void RetiredFrameworkDoesNotSatisfyCoverage()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net7.0", ["Alive.DotNet7_0.verified.txt"], ["Alive"]);
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"], ["Alive"]);

        var merged = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.False(merged.FrameworksCovered);
    }

    /// <summary>
    /// A manifest from before the last build may name tests and files that have since been renamed
    /// or deleted, and counting those as tracked would hide exactly the danglers the check is for.
    /// </summary>
    [Fact]
    public void StaleManifestIsIgnored()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net8.0", ["Stale.DotNet8_0.verified.txt"], ["Stale"]);
        DanglingManifest.Write(directory, "net9.0", ["Alive.DotNet9_0.verified.txt"], ["Alive"]);

        // Pinned rather than left to the clock: the two manifests are written milliseconds apart,
        // and file timestamp granularity is coarser than that on some file systems.
        var buildTime = DateTime.UtcNow;
        File.SetLastWriteTimeUtc(
            DanglingManifest.PathFor(directory, "net8.0"),
            buildTime.AddHours(-1));
        File.SetLastWriteTimeUtc(
            DanglingManifest.PathFor(directory, "net9.0"),
            buildTime.AddHours(1));

        var merged = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", buildTime);

        Assert.False(merged.FrameworksCovered);
        Assert.Equal(["Alive.DotNet9_0.verified.txt"], merged.Files);
        Assert.Equal(["Alive"], merged.Prefixes);
    }

    /// <summary>
    /// A re run of one framework replaces its manifest rather than adding to it, so a file or test
    /// that run no longer tracks stops being tracked.
    /// </summary>
    [Fact]
    public void ReRunReplacesManifest()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Before.verified.txt"], ["Before"]);
        DanglingManifest.Write(directory, "net9.0", ["After.verified.txt"], ["After"]);

        var merged = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.Equal(["After.verified.txt"], merged.Files);
        Assert.Equal(["After"], merged.Prefixes);
    }

    /// <summary>
    /// Several verifications can resolve to one verified path, and every case of a parameterised
    /// test records the same prefix, so both bags hold duplicates.
    /// </summary>
    [Fact]
    public void DuplicatesAreWrittenOnce()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(
            directory,
            "net9.0",
            ["Alive.verified.txt", "Alive.verified.txt"],
            ["Alive", "Alive", "Alive"]);

        var lines = File.ReadAllLines(DanglingManifest.PathFor(directory, "net9.0"));

        Assert.Equal(["[files]", "Alive.verified.txt", "[prefixes]", "Alive"], lines);
    }

    [Fact]
    public void NoDirectoryIsNotCovered()
    {
        using var directory = new TempDirectory();
        var missing = directory.BuildPath("absent");

        var merged = DanglingManifest.Read(missing, ["net9.0"], "net9.0", noStaleFiltering);

        Assert.False(merged.FrameworksCovered);
        Assert.Empty(merged.Files);
        Assert.Empty(merged.Prefixes);
    }

    [Fact]
    public void EmptyManifestIsStillCoverage()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", [], []);

        var merged = DanglingManifest.Read(directory, ["net9.0"], "net9.0", noStaleFiltering);

        Assert.True(merged.FrameworksCovered);
        Assert.Empty(merged.Files);
        Assert.Empty(merged.Prefixes);
    }

    /// <summary>
    /// A test that produced no file at all - inline, excluded targets, or one that threw - records a
    /// prefix and nothing else. That is the case the prefix half exists for.
    /// </summary>
    [Fact]
    public void PrefixWithoutFiles()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", [], ["Inline"]);

        var merged = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.Empty(merged.Files);
        Assert.Equal(["Inline"], merged.Prefixes);
    }

    /// <summary>
    /// A verified path can be anything the file system accepts, so the manifest has to survive
    /// spaces and the characters Verify puts in a name for parameters, targets and indexes.
    /// </summary>
    [Fact]
    public void AwkwardPathsRoundTrip()
    {
        using var directory = new TempDirectory();
        string[] files =
        [
            @"D:\a b\Tests.Method_param=a b.DotNet9_0#00.verified.txt",
            "/home/a b/Tests.Method#name.verified.txt",
            @"D:\a\Tests.Method_param=Ünïcödé.verified.txt"
        ];
        string[] prefixes =
        [
            @"D:\a b\Tests.Method",
            "/home/a b/Tests.Method",
            @"D:\a\[files]Tests.Method"
        ];
        DanglingManifest.Write(directory, "net9.0", files, prefixes);

        var merged = DanglingManifest.Read(directory, [], "net9.0", noStaleFiltering);

        Assert.Equal(files.Order(), merged.Files.Order());
        Assert.Equal(prefixes.Order(), merged.Prefixes.Order());
    }

    /// <summary>
    /// Written through a temporary file and moved into place. A partially written manifest read by
    /// another framework's run would drop paths and report live snapshots as dangling.
    /// </summary>
    [Fact]
    public void WriteLeavesNoTemporaryFiles()
    {
        using var directory = new TempDirectory();
        DanglingManifest.Write(directory, "net9.0", ["Alive.verified.txt"], ["Alive"]);

        var files = Directory.GetFiles(directory)
            .Select(Path.GetFileName)
            .Order();

        Assert.Equal(["net9.0.txt"], files);
    }

    /// <summary>
    /// End to end: two frameworks, one snapshot each, one left behind by a test that was deleted,
    /// and one left behind by a framework that was dropped. The deleted test's file is reported
    /// either way, since no prefix owns it. The retired framework's file needs the complete union:
    /// its test is still there, so only the absence of a run that claims it says anything.
    /// </summary>
    [Fact]
    public Task OrphanAndRetiredFrameworkSurviveTheUnion()
    {
        using var directory = new TempDirectory();
        var root = directory.Path;
        var aliveOnNet8 = Path.Combine(root, "Tests.Alive.DotNet8_0.verified.txt");
        var aliveOnNet9 = Path.Combine(root, "Tests.Alive.DotNet9_0.verified.txt");
        var deletedTest = Path.Combine(root, "Tests.Deleted.DotNet8_0.verified.txt");
        // A runtime no run of this project produces, so the verdict is the same on every target
        // framework and the snapshot can be shared between them.
        var retiredFramework = Path.Combine(root, "Tests.Alive.Mono3_1.verified.txt");
        var alivePrefix = Path.Combine(root, "Tests.Alive");

        DanglingManifest.Write(directory, "net8.0", [aliveOnNet8], [alivePrefix]);
        DanglingManifest.Write(directory, "net9.0", [aliveOnNet9], [alivePrefix]);

        var merged = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.True(merged.FrameworksCovered);
        return Throws(
                () => DanglingSnapshotsCheck.CheckFiles(
                    [aliveOnNet8, aliveOnNet9, deletedTest, retiredFramework],
                    merged.Files,
                    merged.Prefixes,
                    root,
                    merged.FrameworksCovered))
            .IgnoreStackTrace();
    }

    /// <summary>
    /// The same inputs without full coverage. The deleted test's snapshot is still reported, because
    /// no prefix owns it whatever framework it names. The retired framework's one is not: its test
    /// is alive, and without every manifest there is no way to know no run produces it.
    /// </summary>
    [Fact]
    public Task WithoutCoverageOnlyTheOrphanIsReported()
    {
        using var directory = new TempDirectory();
        var root = directory.Path;
        var aliveOnNet9 = Path.Combine(root, "Tests.Alive.DotNet9_0.verified.txt");
        var deletedTest = Path.Combine(root, "Tests.Deleted.DotNet8_0.verified.txt");
        // A runtime no run of this project produces, so the verdict is the same on every target
        // framework and the snapshot can be shared between them.
        var retiredFramework = Path.Combine(root, "Tests.Alive.Mono3_1.verified.txt");
        var alivePrefix = Path.Combine(root, "Tests.Alive");

        DanglingManifest.Write(directory, "net9.0", [aliveOnNet9], [alivePrefix]);

        var merged = DanglingManifest.Read(directory, ["net8.0", "net9.0"], "net9.0", noStaleFiltering);

        Assert.False(merged.FrameworksCovered);
        return Throws(
                () => DanglingSnapshotsCheck.CheckFiles(
                    [aliveOnNet9, deletedTest, retiredFramework],
                    merged.Files,
                    merged.Prefixes,
                    root,
                    merged.FrameworksCovered))
            .IgnoreStackTrace();
    }
}
