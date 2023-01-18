static class MatchingFileFinder
{
    public static IEnumerable<string> FindReceived(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.received.";
        var indexedPattern = $"{fileNamePrefix}#";
        foreach (var file in Directory.EnumerateFiles(directory,  $"{fileNamePrefix}*.received.*"))
        {
            if (file.StartsWith(nonIndexedPattern))
            {
                yield return file;
            }
            if (file.StartsWith(indexedPattern))
            {
                yield return file;
            }
        }
    }
    public static IEnumerable<string> FindVerified(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.verified.";
        var indexedPattern = $"{fileNamePrefix}#";
        foreach (var file in Directory.EnumerateFiles(directory,  $"{fileNamePrefix}*.verified.*"))
        {
            if (file.StartsWith(nonIndexedPattern))
            {
                yield return file;
            }
            if (file.StartsWith(indexedPattern))
            {
                yield return file;
            }
        }
    }
}