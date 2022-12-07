static class MatchingFileFinder
{
    public static IEnumerable<string> Find(string fileNamePrefix, string suffix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.*.*";
        var nonIndexedPrefix = $"{fileNamePrefix}.";
        foreach (var file in Directory.EnumerateFiles(directory, nonIndexedPattern))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.StartsWith(nonIndexedPrefix) && name.EndsWith(suffix))
            {
                yield return file;
            }
        }

        var indexedPattern = $"{fileNamePrefix}#??.*.*";
        var indexedPrefix = $"{fileNamePrefix}#";
        foreach (var file in Directory.EnumerateFiles(directory, indexedPattern))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.StartsWith(indexedPrefix) && name.EndsWith(suffix))
            {
                yield return file;
            }
        }
    }
}