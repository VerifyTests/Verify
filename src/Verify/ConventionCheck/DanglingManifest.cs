namespace VerifyTests;

/// <summary>
/// Records the verified files one target framework's test run tracked, so that the runs of the other
/// target frameworks can read it back.
///
/// Without this a run only knows the files it wrote itself. A multi targeted project runs once per
/// framework, so most of the snapshots on disk belong to a framework that is not running, and the
/// only way to tell those from a snapshot whose test was deleted was to guess from the file name.
/// Unioning the manifests replaces the guess with the answer for that axis.
///
/// Manifests go in the intermediate (obj) directory, below BaseIntermediateOutputPath, which unlike
/// IntermediateOutputPath is shared by every target framework of the project. They are scoped by
/// configuration, since a Debug run says nothing about the snapshots a Release run owns, and named
/// after the target framework, so a re run overwrites rather than accumulates.
///
/// Staleness is bounded two ways: obj is wiped by a clean, and a manifest older than the running
/// assembly is from before the last build and is ignored. A run that cannot account for every target
/// framework says so, and the caller falls back to the name based skip.
/// </summary>
static class DanglingManifest
{
    const string extension = ".txt";

    public static string PathFor(string directory, string targetFramework) =>
        Path.Combine(directory, $"{targetFramework}{extension}");

    /// <summary>
    /// Written through a temporary file and moved into place, so a run of another target framework
    /// reading concurrently sees either the previous manifest or this one, never half of one.
    /// </summary>
    public static void Write(string directory, string targetFramework, IEnumerable<string> trackedFiles)
    {
        Directory.CreateDirectory(directory);
        var path = PathFor(directory, targetFramework);
        var temp = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            File.WriteAllLines(temp, trackedFiles.Distinct());
            File.Move(temp, path, true);
        }
        catch
        {
            IoHelpers.DeleteFile(temp);
            throw;
        }
    }

    /// <summary>
    /// Every verified file recorded by the manifests in <paramref name="directory" />, and whether
    /// those manifests account for every framework in <paramref name="targetFrameworks" />.
    /// </summary>
    /// <param name="staleBefore">
    /// Manifests last written before this are from before the last build and are ignored. The
    /// running assembly's timestamp is the caller's value: a build server builds every framework
    /// before running any of them, so a manifest from this build is always newer.
    /// </param>
    public static (HashSet<string> tracked, bool covered) Read(
        string directory,
        IReadOnlyList<string> targetFrameworks,
        string targetFramework,
        DateTime staleBefore)
    {
        HashSet<string> tracked = [];
        HashSet<string> frameworksRead = new(StringComparer.OrdinalIgnoreCase);

        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.EnumerateFiles(directory, $"*{extension}"))
            {
                if (File.GetLastWriteTimeUtc(file) < staleBefore)
                {
                    continue;
                }

                foreach (var line in File.ReadLines(file))
                {
                    if (line.Length > 0)
                    {
                        tracked.Add(line);
                    }
                }

                frameworksRead.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        return (tracked, IsCovered(targetFrameworks, targetFramework, frameworksRead));
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
