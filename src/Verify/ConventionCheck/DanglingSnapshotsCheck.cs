#pragma warning disable VerifierSettingsTestAssembly
#pragma warning disable CS1998

namespace VerifyTests;

[Experimental("VerifyDanglingSnapshots")]
public static class DanglingSnapshotsCheck
{
    static ConcurrentBag<string>? trackedVerifiedFiles;

    internal static void TrackVerifiedFile(string path) => trackedVerifiedFiles?.Add(path);

    public static void Run()
    {
        if (!BuildServerDetector.Detected)
        {
            return;
        }

        var assembly = VerifierSettings.Assembly;
        var directory = AttributeReader.GetProjectDirectory(assembly);
        var (tracked, frameworksCovered) = MergeWithOtherFrameworks(assembly);
        CheckFiles(FindSnapshotFiles(directory), tracked, directory, frameworksCovered);
    }

    internal static IEnumerable<string> FindSnapshotFiles(string directory) =>
        Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories);

    /// <summary>
    /// Publishes what this run tracked, then reads back what the runs of the other target frameworks
    /// tracked. When every framework is accounted for, the union is the complete set of verified
    /// files the project owns, and a file outside it is dangling whatever its name says.
    /// </summary>
    static (IReadOnlyCollection<string> tracked, bool frameworksCovered) MergeWithOtherFrameworks(Assembly assembly)
    {
        var tracked = trackedVerifiedFiles!;
        var directory = VerifierSettings.DanglingDir;
        var targetFramework = VerifierSettings.TargetFramework;
        if (directory is null ||
            targetFramework is null)
        {
            // The project does not consume Verify's build props, so there is nowhere to put a
            // manifest and no framework list to check it against.
            return (tracked, false);
        }

        try
        {
            DanglingManifest.Write(directory, targetFramework, tracked);
            return DanglingManifest.Read(
                directory,
                VerifierSettings.TargetFrameworks,
                targetFramework,
                AssemblyWriteTime(assembly));
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            // The manifest is an optimization: without it the check still runs, it just falls back
            // to skipping names that look like they belong to another framework. Failing to read or
            // write one must not fail the test run.
            return (tracked, false);
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

        List<string> untracked = [];
        List<string> incorrectCase = [];
        foreach (var file in filesOnDisk)
        {
            if (tracked.Contains(file))
            {
                continue;
            }

            var suffix = file[directory.Length..];
            suffix = suffix.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (IsPlatformUnique(suffix))
            {
                continue;
            }

            if (!frameworksCovered &&
                IsFrameworkUnique(suffix))
            {
                continue;
            }

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
    /// The runtime and target framework axes. A run of one framework never writes the files of
    /// another, so without a manifest from every framework these have to be left alone. With one,
    /// they are decided by the union instead and this is not consulted.
    /// </summary>
    static bool IsFrameworkUnique(string file) =>
        file.Contains(".Net") ||
        file.Contains(".DotNet") ||
        file.Contains(".Mono.");

    /// <summary>
    /// The OS axis. Manifests cannot settle this one: the runs that produce these files are on
    /// other machines, with their own intermediate directories, so their manifests are never
    /// visible here.
    /// </summary>
    static bool IsPlatformUnique(string file) =>
        file.Contains(".OSX.") ||
        file.Contains(".Windows.") ||
        file.Contains(".Linux.");

    [ModuleInitializer]
    internal static void Init()
    {
        if (BuildServerDetector.Detected)
        {
            trackedVerifiedFiles = [];
        }
    }
}
