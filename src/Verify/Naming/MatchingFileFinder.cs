static class MatchingFileFinder
{
    public static IEnumerable<string> Find(string fileNamePrefix, string suffix, string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory, $"{fileNamePrefix}.*.*"))
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