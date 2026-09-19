#pragma warning disable VerifierSettingsTestAssembly
#pragma warning disable CS1998

namespace VerifyTests;

[Experimental("VerifyDanglingSnapshots")]
public static class DanglingSnapshotsCheck
{
    static ConcurrentBag<string>? trackedVerifiedFiles;
    static ConcurrentBag<string>? trackedPrefixes;

    internal static void TrackVerifiedFile(string path) => trackedVerifiedFiles?.Add(path);

    /// <summary>
    /// The verified path up to the end of the type and method, before parameters, uniqueness and
    /// index. Recorded once per verification, from the constructor, so a test is known to exist
    /// even when it produced no file: it was inline, it threw, or every target was excluded.
    /// </summary>
    internal static void TrackPrefix(string prefix) => trackedPrefixes?.Add(prefix);

    public static void Run()
    {
        if (!BuildServerDetector.Detected)
        {
            return;
        }

        if (VerifierSettings.AssemblyOrNull is not { } assembly)
        {
            // No verification ran, so nothing is tracked and every snapshot on disk would be
            // reported. A run that verified nothing is something the test framework reports; adding
            // a second failure here only buries it.
            return;
        }

        var directory = AttributeReader.GetProjectDirectory(assembly);
        var merged = MergeWithOtherFrameworks(assembly);
        CheckFiles(FindSnapshotFiles(directory), merged.Files, merged.Prefixes, directory, merged.FrameworksCovered);
    }

    internal static IEnumerable<string> FindSnapshotFiles(string directory) =>
        Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories);

    /// <summary>
    /// Publishes what this run tracked, then reads back what the runs of the other target frameworks
    /// tracked. When every framework is accounted for, the union is the complete set of verified
    /// files the project owns, and a file outside it is dangling whatever its name says.
    /// </summary>
    static DanglingManifest.Merged MergeWithOtherFrameworks(Assembly assembly)
    {
        var files = trackedVerifiedFiles!;
        var prefixes = trackedPrefixes!;
        var directory = VerifierSettings.DanglingDir;
        var targetFramework = VerifierSettings.TargetFramework;
        if (directory is null ||
            targetFramework is null)
        {
            // The project does not consume Verify's build props, so there is nowhere to put a
            // manifest and no framework list to check it against.
            return new(files, prefixes, false);
        }

        try
        {
            DanglingManifest.Write(directory, targetFramework, files, prefixes);
            return DanglingManifest.Read(
                directory,
                VerifierSettings.TargetFrameworks,
                targetFramework,
                AssemblyWriteTime(assembly));
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            // The manifest is an optimization: without it the check still runs, it just cannot
            // settle the framework axis. Failing to read or write one must not fail the test run.
            return new(files, prefixes, false);
        }
    }

    /// <summary>
    /// A manifest older than the assembly running it is from before the last build, so whatever it
    /// records may since have been renamed or deleted. Unknown timestamp means no filtering, which
    /// is what a single file or in memory assembly gets.
    /// </summary>
    static DateTime AssemblyWriteTime(Assembly assembly)
    {
        var location = assembly.Location;
        if (location.Length == 0)
        {
            return DateTime.MinValue;
        }

        try
        {
            return File.GetLastWriteTimeUtc(location);
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            return DateTime.MinValue;
        }
    }

    internal static void CheckFiles(
        IEnumerable<string> filesOnDisk,
        IReadOnlyCollection<string> trackedFiles,
        IReadOnlyCollection<string> prefixes,
        string directory,
        bool frameworksCovered)
    {
        static void AppendItems(StringBuilder builder, List<string> list, string title)
        {
            if (list.Count <= 0)
            {
                return;
            }

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(title);
            builder.AppendLine();
            foreach (var file in list)
            {
                builder.AppendLine($" * {file}");
            }
        }

        // Materialize the tracked files into sets once. Enumerating a ConcurrentBag
        // (via LINQ Contains) per file on disk snapshots the whole bag each time,
        // making the check O(files * tracked).
        var tracked = new HashSet<string>(trackedFiles);
        var trackedIgnoreCase = new HashSet<string>(trackedFiles, StringComparer.OrdinalIgnoreCase);
        // A prefix identifies a test, and case is not part of that identity. The casing check below
        // is about the snapshot's own name; the directory half of a prefix comes from whatever
        // string each side built its path from, and the two disagree often enough that an ordinal
        // comparison would call every test unknown.
        var knownPrefixes = new HashSet<string>(prefixes, StringComparer.OrdinalIgnoreCase);

        List<string> untracked = [];
        List<string> incorrectCase = [];
        foreach (var file in filesOnDisk)
        {
            if (tracked.Contains(file))
            {
                continue;
            }

            if (!IsDangling(file, knownPrefixes, frameworksCovered))
            {
                continue;
            }

            var suffix = file[directory.Length..];
            suffix = suffix.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (trackedIgnoreCase.Contains(file))
            {
                incorrectCase.Add(suffix);
            }
            else
            {
                untracked.Add(suffix);
            }
        }

        if (untracked.Count == 0 &&
            incorrectCase.Count == 0)
        {
            return;
        }

        var builder = new StringBuilder(
            """

            Verify has detected the following issues with snapshot files:
            """);

        AppendItems(builder, untracked, "The following files have not been tracked:");
        AppendItems(builder, incorrectCase, "The following files have been tracked with incorrect case:");
        throw new(builder.ToString());
    }

    /// <summary>
    /// A file the run did not write is dangling unless some other run owns it.
    ///
    /// When no test of this assembly owns a name starting where this one does, no run of the project
    /// owns it either: the set of tests does not vary by framework, OS or architecture, so there is
    /// nothing left for a name to excuse. That is the whole point of tracking prefixes, and it is
    /// what makes a deleted test's snapshot visible however much uniqueness its name carries.
    ///
    /// When a test does own the name, the file is either a variant this run does not produce, which
    /// the uniqueness segments say, or a leftover, which they do not.
    /// </summary>
    static bool IsDangling(string file, HashSet<string> prefixes, bool frameworksCovered)
    {
        if (prefixes.Count == 0)
        {
            // Nothing recorded the tests, so there is no identity to reason from. Only reachable
            // through a caller that tracks files alone, or a manifest written before prefixes were
            // recorded. Treating every file as unowned would report all of them.
            return false;
        }

        if (!TryMatchPrefix(file, prefixes, out var tailIndex))
        {
            return true;
        }

        return !UniquenessSegments.BelongsToAnotherRun(file[tailIndex..], frameworksCovered);
    }

    /// <summary>
    /// The longest recorded prefix this file continues from. A verified name continues from the type
    /// and method with parameters (<c>_</c>), uniqueness or the verified marker (<c>.</c>), a target
    /// name or index (<c>#</c>), or, for the unique directory conventions, a directory separator.
    /// Probing every such position finds the prefix without having to know which of them the name
    /// uses, and searching from the end takes the longest match.
    /// </summary>
    static bool TryMatchPrefix(string file, HashSet<string> prefixes, out int tailIndex)
    {
        for (var index = file.Length - 1; index > 0; index--)
        {
            var character = file[index];
            if (character is not ('.' or '_' or '#') &&
                character != Path.DirectorySeparatorChar &&
                character != Path.AltDirectorySeparatorChar)
            {
                continue;
            }

            if (prefixes.Contains(file[..index]))
            {
                tailIndex = index;
                return true;
            }
        }

        tailIndex = 0;
        return false;
    }

    [ModuleInitializer]
    internal static void Init()
    {
        if (BuildServerDetector.Detected)
        {
            trackedVerifiedFiles = [];
            trackedPrefixes = [];
        }
    }
}
