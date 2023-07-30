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

    static IEnumerable<string> Find(string directory, string searchPattern, string nonIndexedPattern, string indexedPattern)
    {
        var startIndex = directory.Length + 1;
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
        var slice = value.Slice(start, match.Length);
        return slice.SequenceEqual(match);
    }
}