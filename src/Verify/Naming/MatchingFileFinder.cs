static class MatchingFileFinder
{
    public static void DeleteReceived(string fileNamePrefix, string directory)
    {
        foreach (var file in Find(directory, fileNamePrefix, "received"))
        {
            IoHelpers.DeleteFile(file);
        }
    }

    public static IEnumerable<string> FindVerified(string fileNamePrefix, string directory) =>
        Find(directory, fileNamePrefix, "verified");

    static List<string> Find(string directory, string fileNamePrefix, string marker)
    {
        // Directory.EnumerateFiles inserts a separator only when the directory
        // does not already end with one, so the file-name offset depends on
        // whether the directory has a trailing separator (e.g. UseDirectory("snapshots/")).
        var startIndex = directory.Length;
        if (directory.Length > 0 &&
            directory[^1] != Path.DirectorySeparatorChar &&
            directory[^1] != Path.AltDirectorySeparatorChar)
        {
            startIndex++;
        }

        var indexedPattern = $"{fileNamePrefix}#";
        var nonIndexedPattern = $"{fileNamePrefix}.{marker}.";

        var list = new List<string>();
        var nonIndexedPatternSpan = nonIndexedPattern.AsSpan();
        var indexedPatternSpan = indexedPattern.AsSpan();
        foreach (var file in Directory.EnumerateFiles(directory, $"{fileNamePrefix}*.{marker}.*"))
        {
            var fileSpan = file.AsSpan();
            if (fileSpan.SubStringEquals(nonIndexedPatternSpan, startIndex) ||
                fileSpan.SubStringEquals(indexedPatternSpan, startIndex))
            {
                list.Add(file);
            }
        }

        AddNested(directory, indexedPattern, marker, list);

        return list;
    }

    /// <summary>
    /// A target name can contain a directory separator: VerifyDirectory names each target
    /// after its path within the tree, and SanitizeFilePath deliberately keeps separators.
    /// The file for such a target lives under a `{prefix}#` directory, which the flat scan
    /// cannot see, so without this its verified file is never tracked and a stale one
    /// survives every run.
    /// </summary>
    static void AddNested(string directory, string indexedPattern, string marker, List<string> list)
    {
        var markerSegment = $".{marker}.";
        // The directory name carries the prefix, so everything below it belongs to this
        // test. A neighbouring test that merely starts with the same text has its own
        // `{itsPrefix}#` directory, which this pattern excludes.
        foreach (var subDirectory in Directory.EnumerateDirectories(directory, $"{indexedPattern}*"))
        {
            foreach (var file in Directory.EnumerateFiles(subDirectory, $"*{markerSegment}*", SearchOption.AllDirectories))
            {
                // The Win32 pattern also matches a name ending in `.verified` with no
                // extension, which is not a file Verify writes.
                if (Path.GetFileName(file)
                    .Contains(markerSegment))
                {
                    list.Add(file);
                }
            }
        }
    }

    static bool SubStringEquals(this CharSpan value, CharSpan match, int start)
    {
        // The Win32 search pattern `{prefix}*.received.*` also matches a stray file named
        // exactly `{prefix}.received` (a trailing `.*` matches "no extension"), which is
        // shorter than the pattern being compared against.
        if (value.Length < start + match.Length)
        {
            return false;
        }

        var slice = value.Slice(start, match.Length);
        return slice.SequenceEqual(match);
    }
}
