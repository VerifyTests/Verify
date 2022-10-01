static class MatchingFileFinder
{
    public static IEnumerable<string> Find(string fileNamePrefix, string suffix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.*.*";
        var indexedPattern = $"{fileNamePrefix}#??.*.*";
        var files = Directory.EnumerateFiles(directory, nonIndexedPattern)
            .Concat(Directory.EnumerateFiles(directory, indexedPattern));
        foreach (var file in files)
        {
            if (ShouldInclude(fileNamePrefix, suffix, file))
            {
                yield return file;
            }
        }
    }

    public static bool ShouldInclude(string fileNamePrefix, string suffix, string file)
    {
        var name = Path.GetFileNameWithoutExtension(file);

        return (name.StartsWith(fileNamePrefix + ".") ||
                name.StartsWith(fileNamePrefix + '#')) &&
               name.EndsWith(suffix);
    }
}