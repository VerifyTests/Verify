#pragma warning disable VerifyDanglingSnapshots
public class DanglingSnapshotsCheckTests
{
    const string root = "path/to";

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

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, root))
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

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, root))
            .IgnoreStackTrace();
    }

    [Fact]
    public void AllTracked()
    {
        var filesOnDisk = new List<string> { "path/to/tracked.verified.txt" };
        var trackedFiles = new ConcurrentBag<string> { "path/to/tracked.verified.txt" };

        DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, root);
    }

    /// <summary>
    /// Every file here is a snapshot for a test that no longer exists: nothing shares its prefix
    /// with anything the run tracked. A file is reported only when its whole relative path avoids
    /// the <c>IfFileUnique</c> substring list, so the ones carrying a runtime, framework or OS
    /// segment are skipped. They are skipped on every framework and every OS, so nothing ever
    /// reports them.
    /// </summary>
    [Fact]
    public Task OrphanedWithUniqueness() =>
        Verify(
            Report(
                [
                    "path/to/Deleted.verified.txt",
                    "path/to/Deleted.DotNet.verified.txt",
                    "path/to/Deleted.DotNet9_0.verified.txt",
                    "path/to/Deleted.Net.verified.txt",
                    "path/to/Deleted.Net4_8.verified.txt",
                    "path/to/Deleted.Mono.verified.txt",
                    "path/to/Deleted.Mono6_12.verified.txt",
                    "path/to/Deleted.Windows.verified.txt",
                    "path/to/Deleted.Linux.verified.txt",
                    "path/to/Deleted.OSX.verified.txt",
                    "path/to/Deleted.Android.verified.txt",
                    "path/to/Deleted.IOS.verified.txt",
                    "path/to/Deleted.x64.verified.txt",
                    "path/to/Deleted.arm64.verified.txt",
                    "path/to/Deleted.Debug.verified.txt",
                    "path/to/Deleted.Release.verified.txt"
                ],
                "path/to/Alive.verified.txt"));

    /// <summary>
    /// The case the uniqueness skip exists for. A multi targeted project running one framework
    /// leaves the snapshots of the other frameworks untouched, and the same holds for the OS,
    /// architecture and configuration axes. None of these may be reported.
    /// </summary>
    [Fact]
    public Task UniquenessFromAnotherRun() =>
        Verify(
            Report(
                [
                    "path/to/Alive.DotNet9_0.verified.txt",
                    "path/to/Alive.DotNet8_0.verified.txt",
                    "path/to/Alive.Net4_8.verified.txt",
                    "path/to/Alive.Linux.verified.txt",
                    "path/to/Alive.arm64.verified.txt",
                    "path/to/Alive.Debug.verified.txt"
                ],
                "path/to/Alive.DotNet9_0.verified.txt"));

    /// <summary>
    /// <c>IfFileUnique</c> matches <c>.Net</c> with no trailing separator, so any name that merely
    /// starts a word with <c>Net</c> is skipped. None of these are uniqueness segments, and all of
    /// them are snapshots for tests that do not exist.
    /// </summary>
    [Fact]
    public Task OrphanedNamesContainingNet() =>
        Verify(
            Report(
                [
                    "path/to/Tests.NetworkClient.verified.txt",
                    "path/to/Tests.NetCoreShim.verified.txt",
                    "path/to/Tests.Nettle.verified.txt",
                    "path/to/HttpTests.NetworkFailure.verified.txt"
                ],
                "path/to/Alive.verified.txt"));

    /// <summary>
    /// The substring list is applied to the path relative to the project, not to the file name, so
    /// a single directory whose name contains one of the tokens hides every snapshot below it.
    /// </summary>
    [Fact]
    public Task OrphanedUnderDirectoryContainingUniquenessToken() =>
        Verify(
            Report(
                [
                    "path/to/Shared.NetCore/Deleted.verified.txt",
                    "path/to/Snapshots.Windows.Only/Deleted.verified.txt",
                    "path/to/Plain/Deleted.verified.txt"
                ],
                "path/to/Alive.verified.txt"));

    /// <summary>
    /// The skip is a case sensitive <c>Contains</c>, so a segment is skipped only in the exact
    /// casing <c>Namer</c> produces. Not wrong on its own, but it shows the list is matching text
    /// rather than recognising a uniqueness value.
    /// </summary>
    [Fact]
    public Task OrphanedWithLowerCaseUniquenessSegments() =>
        Verify(
            Report(
                [
                    "path/to/Deleted.windows.verified.txt",
                    "path/to/Deleted.linux.verified.txt",
                    "path/to/Deleted.osx.verified.txt",
                    "path/to/Deleted.dotnet9_0.verified.txt"
                ],
                "path/to/Alive.verified.txt"));

    /// <summary>
    /// A verified name is <c>{Type}.{Method}{_parameters}{.uniqueness}#{index}</c>. Everything
    /// after the type and method varies per run, so none of it identifies the test.
    /// </summary>
    [Fact]
    public Task OrphanedWithIndexesAndParameters() =>
        Verify(
            Report(
                [
                    "path/to/Deleted#00.verified.txt",
                    "path/to/Deleted#name.verified.txt",
                    "path/to/Deleted_param=value.verified.txt",
                    "path/to/Deleted_param=value.DotNet9_0#00.verified.txt",
                    "path/to/Deleted.DotNet9_0#00.verified.txt"
                ],
                "path/to/Alive.verified.txt"));

    /// <summary>
    /// A test that is still there but whose parameter set changed leaves a snapshot behind. The
    /// prefix is live, so this is the one case where the run genuinely cannot tell a stale
    /// parameter from one produced by a sibling test case that did not run.
    /// </summary>
    [Fact]
    public Task StaleParameterOnLiveTest() =>
        Verify(
            Report(
                [
                    "path/to/Alive_param=current.verified.txt",
                    "path/to/Alive_param=removed.verified.txt"
                ],
                "path/to/Alive_param=current.verified.txt"));

    /// <summary>
    /// <c>ResolveDirectory</c> combines the source file directory with a relative
    /// <c>UseDirectory</c> through <c>Path.Combine</c>, which does not normalise, so the tracked
    /// path keeps its <c>..</c>. The scan returns the normalised path and the ordinal comparison
    /// misses, reporting a live snapshot as dangling.
    /// </summary>
    [Fact]
    public Task UnnormalizedTrackedPath() =>
        Verify(
            Report(
                ["path/to/Snapshots/Alive.verified.txt"],
                "path/to/Tests/../Snapshots/Alive.verified.txt"));

    /// <summary>
    /// Only the case of the file name is meaningful. The case of the directory comes from whatever
    /// string each side happened to build its path from: the scan starts at the project directory
    /// recorded by MSBuild, the tracked path starts at the compiler's caller file path.
    /// </summary>
    [Fact]
    public Task DirectoryCaseMismatch() =>
        Verify(
            Report(
                ["path/to/Nested/Alive.verified.txt"],
                "path/to/nested/Alive.verified.txt"));

    /// <summary>
    /// The project directory recorded by MSBuild ends with a separator, so the relative suffix has
    /// to survive both forms.
    /// </summary>
    [Fact]
    public Task DirectoryWithTrailingSeparator() =>
        Verify(
            ReportIn(
                "path/to/",
                ["path/to/Deleted.verified.txt"],
                "path/to/Alive.verified.txt"));

    [Fact]
    public Task UntrackedAndIncorrectCase() =>
        Verify(
            Report(
                [
                    "path/to/Deleted.verified.txt",
                    "path/to/AliveWithCase.verified.txt"
                ],
                "path/to/alivewithcase.verified.txt"));

    /// <summary>
    /// A test can track a verified path that has no file yet: the first run of a new test, or a run
    /// where every target was new. That is not a dangling file.
    /// </summary>
    [Fact]
    public Task TrackedFileMissingFromDisk() =>
        Verify(Report([], "path/to/NotYetAccepted.verified.txt"));

    /// <summary>
    /// Several verifications can resolve to one verified path, through <c>UseFileName</c> or a
    /// <c>DerivePathInfo</c> that collapses them, so the bag can hold duplicates.
    /// </summary>
    [Fact]
    public Task DuplicateTrackedEntries() =>
        Verify(
            Report(
                ["path/to/Alive.verified.txt"],
                "path/to/Alive.verified.txt",
                "path/to/Alive.verified.txt"));

    [Fact]
    public Task NothingOnDisk() =>
        Verify(Report([]));

    /// <summary>
    /// <c>UseUniqueDirectory</c> in split mode writes to
    /// <c>{prefix}.verified\{target}.{extension}</c>, so <c>.verified.</c> is in the directory name
    /// rather than the file name and the scan's glob never matches. A deleted test leaves its whole
    /// snapshot directory behind.
    /// </summary>
    [Fact]
    public void FindSplitModeUniqueDirectory()
    {
        using var directory = new TempDirectory();
        WriteNested(directory, "Tests.Split.verified", "target.txt");
        WriteNested(directory, "Tests.Split.verified", "target#00.txt");
        WriteNested(directory, "Tests.Split.verified", "nested", "target.txt");

        Assert.Empty(DanglingSnapshotsCheck.FindSnapshotFiles(directory));
    }

    /// <summary>
    /// The non split unique directory convention keeps <c>.verified.</c> in the file name, so those
    /// files are found.
    /// </summary>
    [Fact]
    public void FindNonSplitUniqueDirectory()
    {
        using var directory = new TempDirectory();
        WriteNested(directory, "Tests.Unique", "target.verified.txt");
        WriteNested(directory, "Tests.Unique", "target#00.verified.txt");

        Assert.Equal(2, DanglingSnapshotsCheck.FindSnapshotFiles(directory).Count());
    }

    /// <summary>
    /// The scan walks every directory below the project, build output included. Harmless while
    /// nothing copies snapshots there, and a report of files that are not snapshot sources for
    /// every project that sets <c>CopyToOutputDirectory</c> on them.
    /// </summary>
    [Fact]
    public void FindIncludesBinAndObj()
    {
        using var directory = new TempDirectory();
        WriteNested(directory, "bin", "Release", "net9.0", "Tests.Copied.verified.txt");
        WriteNested(directory, "obj", "Release", "net9.0", "Tests.Copied.verified.txt");

        Assert.Equal(2, DanglingSnapshotsCheck.FindSnapshotFiles(directory).Count());
    }

    [Fact]
    public void FindExcludesReceived()
    {
        using var directory = new TempDirectory();
        WriteNested(directory, "Tests.Alive.received.txt");
        WriteNested(directory, "Tests.Alive.verified.txt");

        var found = DanglingSnapshotsCheck.FindSnapshotFiles(directory)
            .ToList();

        Assert.Single(found);
        Assert.EndsWith("Tests.Alive.verified.txt", found[0]);
    }

    /// <summary>
    /// The Win32 glob <c>*.verified.*</c> also matches a name ending in <c>.verified</c> with no
    /// extension, which is not a file Verify writes.
    /// </summary>
    [Fact]
    public void FindIncludesExtensionlessVerified()
    {
        using var directory = new TempDirectory();
        WriteNested(directory, "Tests.Stray.verified");

        Assert.Single(DanglingSnapshotsCheck.FindSnapshotFiles(directory));
    }

    [Fact]
    public void FindIncludesNestedDirectories()
    {
        using var directory = new TempDirectory();
        WriteNested(directory, "Tests.Alive.verified.txt");
        WriteNested(directory, "Snapshots", "Tests.Alive.verified.txt");
        WriteNested(directory, "Snapshots", "Deep", "Tests.Alive.verified.txt");

        Assert.Equal(3, DanglingSnapshotsCheck.FindSnapshotFiles(directory).Count());
    }

    static string WriteNested(TempDirectory directory, params string[] segments)
    {
        var path = directory.BuildPath(segments);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "content");
        return path;
    }

    static string Report(IEnumerable<string> filesOnDisk, params string[] tracked) =>
        ReportIn(root, filesOnDisk, tracked);

    static string ReportIn(string directory, IEnumerable<string> filesOnDisk, params string[] tracked)
    {
        ConcurrentBag<string> trackedFiles = [..tracked];
        try
        {
            DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, directory);
            return "Nothing reported.";
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }
}
