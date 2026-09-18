#pragma warning disable VerifyDanglingSnapshots
public class DanglingSnapshotsCheckTests
{
    const string root = "path/to";

    // Runtimes and architectures no run of this project produces, so the classifier reaches the same
    // verdict on every target framework and machine and the snapshots can be shared between them.
    // A test that hard coded, say, DotNet8_0 would report it as belonging to another run everywhere
    // except the net8.0 run, which owns it.
    const string foreignRuntime = "Mono3_1";
    const string otherForeignRuntime = "Mono4_2";
    const string foreignArchitecture = "s390x";

    static readonly string[] alive = ["path/to/Alive.verified.txt"];
    static readonly string[] alivePrefix = ["path/to/Alive"];

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

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, ["path/to/tracked"], root, false))
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

        return Throws(() => DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, ["path/to/tracked"], root, false))
            .IgnoreStackTrace();
    }

    [Fact]
    public void AllTracked()
    {
        var filesOnDisk = new List<string> { "path/to/tracked.verified.txt" };
        var trackedFiles = new ConcurrentBag<string> { "path/to/tracked.verified.txt" };

        DanglingSnapshotsCheck.CheckFiles(filesOnDisk, trackedFiles, ["path/to/tracked"], root, false);
    }

    /// <summary>
    /// Every file here is a snapshot for a test that no longer exists: no recorded prefix owns a
    /// name starting where theirs do. The set of tests does not vary by framework, OS or
    /// architecture, so there is no run left for the uniqueness in the name to belong to, and all of
    /// them are reported whether or not the manifests cover the frameworks.
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
                alive,
                alivePrefix));

    /// <summary>
    /// The axes no manifest can settle. The architecture one is skipped whether or not the
    /// frameworks are covered, since the run that owns it is on another machine. The configuration
    /// one has no enumerable value set, so it is not recognised as uniqueness at all and is
    /// reported.
    /// </summary>
    [Fact]
    public Task UniquenessFromAnotherRun() =>
        Verify(
            Report(
                [
                    $"path/to/Alive.{foreignArchitecture}.verified.txt",
                    "path/to/Alive.Debug.verified.txt"
                ],
                alive,
                alivePrefix));

    /// <summary>
    /// A project that dropped a target framework keeps the snapshots that framework owned. The test
    /// is still there, so only a complete union can say the file is stale: by name it is
    /// indistinguishable from the file of a framework that simply is not running.
    /// </summary>
    [Fact]
    public Task RetiredFrameworkSnapshots() =>
        Verify(
            Report(
                [
                    $"path/to/Alive.{foreignRuntime}.verified.txt",
                    $"path/to/Alive.{otherForeignRuntime}.verified.txt"
                ],
                [$"path/to/Alive.{foreignRuntime}.verified.txt"],
                alivePrefix));

    /// <summary>
    /// Names that merely start a word with <c>Net</c>. A whole segment match cannot confuse them
    /// with a uniqueness segment, and none of them is owned by a live test.
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
                alive,
                alivePrefix));

    /// <summary>
    /// A directory whose name contains a uniqueness token holds snapshots like any other. Only the
    /// segments after a recorded prefix are uniqueness, so the directory name cannot hide them.
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
                alive,
                alivePrefix));

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
                alive,
                alivePrefix));

    /// <summary>
    /// A verified name is <c>{Type}.{Method}{_parameters}{.uniqueness}#{index}</c>. Everything after
    /// the type and method varies per run, so none of it identifies the test.
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
                alive,
                alivePrefix));

    /// <summary>
    /// A test that is still there but whose parameter set changed leaves a snapshot behind. The
    /// prefix is live and no uniqueness segment excuses the file, so it is reported.
    /// </summary>
    [Fact]
    public Task StaleParameterOnLiveTest() =>
        Verify(
            Report(
                [
                    "path/to/Alive_param=current.verified.txt",
                    "path/to/Alive_param=removed.verified.txt"
                ],
                ["path/to/Alive_param=current.verified.txt"],
                alivePrefix));

    /// <summary>
    /// A prefix ends at the type and method, so a test whose name merely starts with another's does
    /// not lend it its identity.
    /// </summary>
    [Fact]
    public Task PrefixIsNotASubstringMatch() =>
        Verify(
            Report(
                [
                    "path/to/Alive.verified.txt",
                    "path/to/AliveToo.verified.txt",
                    $"path/to/AliveToo.{foreignRuntime}.verified.txt"
                ],
                alive,
                alivePrefix));

    /// <summary>
    /// Two tests where one name extends the other. The longer prefix has to win, or the shorter
    /// one's uniqueness rules would be applied to the longer one's files.
    /// </summary>
    [Fact]
    public Task LongestPrefixWins() =>
        Verify(
            Report(
                [
                    "path/to/Tests.Method.verified.txt",
                    "path/to/Tests.Method.Nested.verified.txt",
                    $"path/to/Tests.Method.Nested.{foreignRuntime}.verified.txt"
                ],
                ["path/to/Tests.Method.verified.txt"],
                ["path/to/Tests.Method", "path/to/Tests.Method.Nested"]));

    /// <summary>
    /// The directory half of a prefix comes from the compiler's caller file path, while the scan
    /// starts at the project directory MSBuild recorded. The two disagree on case often enough that
    /// matching them ordinally would call every test unknown.
    /// </summary>
    [Fact]
    public Task PrefixMatchIgnoresCase() =>
        Verify(
            Report(
                [$"path/to/Nested/Alive.{otherForeignRuntime}.verified.txt"],
                [$"path/to/Nested/Alive.{foreignRuntime}.verified.txt"],
                ["path/to/nested/Alive"]));

    /// <summary>
    /// The unique directory conventions put the parameters and uniqueness in a directory name, with
    /// the snapshots below it, so a prefix is followed by a separator rather than by a dot.
    /// </summary>
    [Fact]
    public Task UniqueDirectoryPrefixes() =>
        Verify(
            Report(
                [
                    "path/to/Alive/target.verified.txt",
                    $"path/to/Alive.{foreignRuntime}/target.verified.txt",
                    $"path/to/Deleted.{foreignRuntime}/target.verified.txt"
                ],
                [
                    "path/to/Alive/target.verified.txt",
                    $"path/to/Alive.{foreignRuntime}/target.verified.txt"
                ],
                alivePrefix));

    /// <summary>
    /// <c>UseFileName</c> pins the verified name, so the recorded prefix is that name rather than
    /// the type and method.
    /// </summary>
    [Fact]
    public Task UseFileNamePrefix() =>
        Verify(
            Report(
                [
                    "path/to/Pinned.verified.txt",
                    $"path/to/Pinned.{foreignRuntime}.verified.txt",
                    $"path/to/Unpinned.{foreignRuntime}.verified.txt"
                ],
                ["path/to/Pinned.verified.txt"],
                ["path/to/Pinned"]));

    /// <summary>
    /// Nothing recorded the tests, so there is no identity to reason from and every file would look
    /// unowned. Only reachable through a caller that tracks files alone, or a manifest written
    /// before prefixes were recorded.
    /// </summary>
    [Fact]
    public Task NoPrefixesRecorded() =>
        Verify(
            Report(
                [
                    "path/to/Deleted.verified.txt",
                    $"path/to/Deleted.{foreignRuntime}.verified.txt"
                ],
                alive,
                []));

    /// <summary>
    /// <c>ResolveDirectory</c> combines the source file directory with a relative
    /// <c>UseDirectory</c> through <c>Path.Combine</c>, which does not normalise, so the tracked
    /// path keeps its <c>..</c>. The scan returns the normalised path, and neither the file nor the
    /// prefix matches it.
    /// </summary>
    [Fact]
    public Task UnnormalizedTrackedPath() =>
        Verify(
            Report(
                ["path/to/Snapshots/Alive.verified.txt"],
                ["path/to/Tests/../Snapshots/Alive.verified.txt"],
                ["path/to/Tests/../Snapshots/Alive"]));

    /// <summary>
    /// Only the case of the file name is meaningful. The prefix matches either way, so the file
    /// reaches the casing check rather than being reported as an unknown test.
    /// </summary>
    [Fact]
    public Task DirectoryCaseMismatch() =>
        Verify(
            Report(
                ["path/to/Nested/Alive.verified.txt"],
                ["path/to/nested/Alive.verified.txt"],
                ["path/to/nested/Alive"]));

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
                alive,
                alivePrefix));

    [Fact]
    public Task UntrackedAndIncorrectCase() =>
        Verify(
            Report(
                [
                    "path/to/Deleted.verified.txt",
                    "path/to/AliveWithCase.verified.txt"
                ],
                ["path/to/alivewithcase.verified.txt"],
                ["path/to/alivewithcase"]));

    /// <summary>
    /// A test can track a verified path that has no file yet: the first run of a new test, or a run
    /// where every target was new. That is not a dangling file.
    /// </summary>
    [Fact]
    public Task TrackedFileMissingFromDisk() =>
        Verify(Report([], ["path/to/NotYetAccepted.verified.txt"], ["path/to/NotYetAccepted"]));

    /// <summary>
    /// Several verifications can resolve to one verified path, through <c>UseFileName</c> or a
    /// <c>DerivePathInfo</c> that collapses them, so the bag can hold duplicates.
    /// </summary>
    [Fact]
    public Task DuplicateTrackedEntries() =>
        Verify(
            Report(
                ["path/to/Alive.verified.txt"],
                ["path/to/Alive.verified.txt", "path/to/Alive.verified.txt"],
                ["path/to/Alive", "path/to/Alive"]));

    [Fact]
    public Task NothingOnDisk() =>
        Verify(Report([], alive, alivePrefix));

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

    static string Report(IEnumerable<string> filesOnDisk, IReadOnlyCollection<string> tracked, IReadOnlyCollection<string> prefixes) =>
        ReportIn(root, filesOnDisk, tracked, prefixes);

    /// <summary>
    /// Reports both ways round. Without a manifest from every target framework the check leaves the
    /// runtime and framework axis to the name; with one the union decides it instead, and the
    /// difference between the two halves is what the manifests bought.
    /// </summary>
    static string ReportIn(string directory, IEnumerable<string> filesOnDisk, IReadOnlyCollection<string> tracked, IReadOnlyCollection<string> prefixes)
    {
        var files = filesOnDisk.ToList();
        var builder = new StringBuilder();
        builder.AppendLine("== Frameworks not covered ==");
        builder.AppendLine(Check(files, tracked, prefixes, directory, false));
        builder.AppendLine("== Frameworks covered ==");
        builder.Append(Check(files, tracked, prefixes, directory, true));
        return builder.ToString();
    }

    static string Check(
        IReadOnlyCollection<string> filesOnDisk,
        IReadOnlyCollection<string> tracked,
        IReadOnlyCollection<string> prefixes,
        string directory,
        bool frameworksCovered)
    {
        try
        {
            DanglingSnapshotsCheck.CheckFiles(filesOnDisk, tracked, prefixes, directory, frameworksCovered);
            return "Nothing reported.";
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }
}
