static class MatchingFileFinder
{
    public static IEnumerable<string> FindReceived(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.received.";
        var indexedPattern = $"{fileNamePrefix}#";
        var startIndex = directory.Length + 1;
        foreach (var file in Directory.GetFiles(directory, $"{fileNamePrefix}*.received.*"))
        {
            if (file.SubStringEquals(nonIndexedPattern, startIndex))
            {
                yield return file;
            }
            else if (file.SubStringEquals(indexedPattern, startIndex))
            {
                yield return file;
            }
        }
    }

    public static IEnumerable<string> FindVerified(string fileNamePrefix, string directory)
    {
        var nonIndexedPattern = $"{fileNamePrefix}.verified.";
        var indexedPattern = $"{fileNamePrefix}#";
        var startIndex = directory.Length + 1;
        foreach (var file in Directory.GetFiles(directory, $"{fileNamePrefix}*.verified.*"))
        {
            if (file.SubStringEquals(nonIndexedPattern, startIndex))
            {
                yield return file;
            }
            else if (file.SubStringEquals(indexedPattern, startIndex))
            {
                yield return file;
            }
        }
    }

    static bool SubStringEquals(this string value, string match, int startIndex)
    {
        for (var i = 0; i < match.Length; i++)
        {
            if (value[startIndex + i] != match[i])
            {
                return false;
            }
        }

        return true;
    }
}