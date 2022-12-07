static class MatchingFileFinder
{
    public static IEnumerable<string> FindReceived(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.received.*";
        foreach (var file in Directory.GetFiles(directory, nonIndexedPattern))
        {
            yield return file;
        }

        var indexedPattern = $"{fileNamePrefix}#??.received.*";
        foreach (var file in Directory.EnumerateFiles(directory, indexedPattern))
        {
            yield return file;
        }
    }

    public static IEnumerable<string> FindVerified(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.verified.*";
        foreach (var file in Directory.GetFiles(directory, nonIndexedPattern))
        {
            yield return file;
        }

        var indexedPattern = $"{fileNamePrefix}#??.verified.*";
        foreach (var file in Directory.EnumerateFiles(directory, indexedPattern))
        {
            yield return file;
        }
    }
}