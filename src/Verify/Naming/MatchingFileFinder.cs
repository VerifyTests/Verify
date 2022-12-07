static class MatchingFileFinder
{
    public static IEnumerable<string> Find(string fileNamePrefix, string suffix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.*.*";
        foreach (var file in Directory.GetFiles(directory, nonIndexedPattern))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.EndsWith(suffix))
            {
                yield return file;
            }
        }

        var indexedPattern = $"{fileNamePrefix}#??.*.*";
        foreach (var file in Directory.EnumerateFiles(directory, indexedPattern))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.EndsWith(suffix))
            {
                yield return file;
            }
        }
    }
    public static IEnumerable<string> FindVerified(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.*.*";
        foreach (var file in Directory.GetFiles(directory, nonIndexedPattern))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.EndsWith(".verified"))
            {
                yield return file;
            }
        }

        var indexedPattern = $"{fileNamePrefix}#??.*.*";
        foreach (var file in Directory.EnumerateFiles(directory, indexedPattern))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.EndsWith(".verified"))
            {
                yield return file;
            }
        }
    }
}