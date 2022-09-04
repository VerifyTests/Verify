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
        if (!name.EndsWith(suffix))
        {
            return false;
        }

        var nameWithoutSuffix = name[..^suffix.Length];
        if (nameWithoutSuffix == fileNamePrefix)
        {
            return true;
        }

        var nameLength = nameWithoutSuffix.Length - fileNamePrefix.Length;
        var potentialIndexPart = nameWithoutSuffix
            .Substring(fileNamePrefix.Length, nameLength);

        if (potentialIndexPart.StartsWith('#'))
        {
            var potentialPrefix = nameWithoutSuffix[..^potentialIndexPart.Length];
            return fileNamePrefix == potentialPrefix;
        }

        if (potentialIndexPart.StartsWith('.'))
        {
            if (ushort.TryParse(new(potentialIndexPart.Skip(1).Take(2).ToArray()), out _))
            {
                var potentialPrefix = nameWithoutSuffix[..^potentialIndexPart.Length];
                return fileNamePrefix == potentialPrefix;
            }
        }

        return false;
    }
}