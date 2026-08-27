static class MatchingFileFinder
{
    public static void DeleteReceived(string fileNamePrefix, string directory)
    {
        foreach (var file in Find(
                     directory,
                     searchPattern: $"{fileNamePrefix}*.received.*",
                     nonIndexedPattern: $"{fileNamePrefix}.received.",
                     indexedPattern: $"{fileNamePrefix}#"))
        {
            IoHelpers.DeleteFile(file);
        }
    }

    public static IEnumerable<string> FindVerified(string fileNamePrefix, string directory) =>
        Find(
            directory,
            searchPattern: $"{fileNamePrefix}*.verified.*",
            nonIndexedPattern: $"{fileNamePrefix}.verified.",
            indexedPattern: $"{fileNamePrefix}#");

    static List<string> Find(string directory, string searchPattern, string nonIndexedPattern, string indexedPattern)
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

        var list = new List<string>();
        var nonIndexedPatternSpan = nonIndexedPattern.AsSpan();
        var indexedPatternSpan = indexedPattern.AsSpan();
        foreach (var file in Directory.EnumerateFiles(directory, searchPattern))
        {
            var fileSpan = file.AsSpan();
            if (fileSpan.SubStringEquals(nonIndexedPatternSpan, startIndex) ||
                fileSpan.SubStringEquals(indexedPatternSpan, startIndex))
            {
                list.Add(file);
            }
        }

        return list;
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