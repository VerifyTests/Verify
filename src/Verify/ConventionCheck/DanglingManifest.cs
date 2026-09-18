namespace VerifyTests;

/// <summary>
/// Records what one target framework's test run tracked, so that the runs of the other target
/// frameworks can read it back.
///
/// Without this a run only knows what it did itself. A multi targeted project runs once per
/// framework, so most of the snapshots on disk belong to a framework that is not running, and the
/// only way to tell those from a snapshot whose test was deleted was to guess from the file name.
/// Unioning the manifests replaces the guess with the answer for that axis.
///
/// Both halves are recorded. The files say which snapshots exist; the prefixes say which tests do,
/// which matters just as much across frameworks: a test behind a <c>#if NET48</c> exists only in
/// the net48 run, and without its prefix every other run would call its snapshots unowned.
///
/// Manifests go in the intermediate (obj) directory, below BaseIntermediateOutputPath, which unlike
/// IntermediateOutputPath is shared by every target framework of the project. They are scoped by
/// configuration, since a Debug run says nothing about the snapshots a Release run owns, and named
/// after the target framework, so a re run overwrites rather than accumulates.
///
/// Staleness is bounded two ways: obj is wiped by a clean, and a manifest older than the running
/// assembly is from before the last build and is ignored. A run that cannot account for every target
/// framework says so, and the caller leaves the framework axis to the name instead.
/// </summary>
static class DanglingManifest
{
    const string extension = ".txt";
    const string filesHeader = "[files]";
    const string prefixesHeader = "[prefixes]";

    internal readonly struct Merged(IReadOnlyCollection<string> files, IReadOnlyCollection<string> prefixes, bool frameworksCovered)
    {
        public IReadOnlyCollection<string> Files { get; } = files;
        public IReadOnlyCollection<string> Prefixes { get; } = prefixes;
        public bool FrameworksCovered { get; } = frameworksCovered;
    }

    public static string PathFor(string directory, string targetFramework) =>
        Path.Combine(directory, $"{targetFramework}{extension}");

    /// <summary>
    /// Written through a temporary file and moved into place, so a run of another target framework
    /// reading concurrently sees either the previous manifest or this one, never half of one.
    /// </summary>
    public static void Write(
        string directory,
        string targetFramework,
        IEnumerable<string> files,
        IEnumerable<string> prefixes)
    {
        Directory.CreateDirectory(directory);
        var path = PathFor(directory, targetFramework);
        var temp = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            // The headers cannot collide with a recorded path: both halves are absolute, so neither
            // can be a line that is exactly "[files]" or "[prefixes]".
            List<string> lines = [filesHeader, ..files.Distinct(), prefixesHeader, ..prefixes.Distinct()];
            File.WriteAllLines(temp, lines);
            File.Move(temp, path, true);
        }
        catch
        {
            IoHelpers.DeleteFile(temp);
            throw;
        }
    }

    /// <summary>
    /// Everything recorded by the manifests in <paramref name="directory" />, and whether those
    /// manifests account for every framework in <paramref name="targetFrameworks" />.
    /// </summary>
    /// <param name="staleBefore">
    /// Manifests last written before this are from before the last build and are ignored. The
    /// running assembly's timestamp is the caller's value: a build server builds every framework
    /// before running any of them, so a manifest from this build is always newer.
    /// </param>
    public static Merged Read(
        string directory,
        IReadOnlyList<string> targetFrameworks,
        string targetFramework,
        DateTime staleBefore)
    {
        HashSet<string> files = [];
        HashSet<string> prefixes = [];
        HashSet<string> frameworksRead = new(StringComparer.OrdinalIgnoreCase);

        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.EnumerateFiles(directory, $"*{extension}"))
            {
                if (File.GetLastWriteTimeUtc(file) < staleBefore)
                {
                    continue;
                }

                ReadInto(file, files, prefixes);
                frameworksRead.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        return new(files, prefixes, IsCovered(targetFrameworks, targetFramework, frameworksRead));
    }

    static void ReadInto(string file, HashSet<string> files, HashSet<string> prefixes)
    {
        var target = files;
        foreach (var line in File.ReadLines(file))
        {
            if (line.Length == 0)
            {
                continue;
            }

            if (line == filesHeader)
            {
                target = files;
                continue;
            }

            if (line == prefixesHeader)
            {
                target = prefixes;
                continue;
            }

            target.Add(line);
        }
    }

    /// <summary>
    /// A single targeted project has no TargetFrameworks value, so its own manifest is the whole
    /// picture. A multi targeted project needs one manifest per framework: anything less and the
    /// files of the frameworks still to run are indistinguishable from dangling ones.
    /// </summary>
    static bool IsCovered(IReadOnlyList<string> targetFrameworks, string targetFramework, HashSet<string> frameworksRead)
    {
        if (targetFrameworks.Count == 0)
        {
            return frameworksRead.Contains(targetFramework);
        }

        foreach (var framework in targetFrameworks)
        {
            if (!frameworksRead.Contains(framework))
            {
                return false;
            }
        }

        return true;
    }
}
