static class MatchingFileFinder
{
    public static IEnumerable<string> Find(string fileNamePrefix, string suffix, string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory, $"{fileNamePrefix}.*.*"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (!name.EndsWith(suffix))
            {
                continue;
            }

            var nameLength = name.Length - fileNamePrefix.Length;
            var prefixRemoved = name
                .Substring(fileNamePrefix.Length, nameLength);
            if (prefixRemoved == suffix)
            {
                yield return file;
                continue;
            }

            var numberPart = prefixRemoved
                .Substring(1, prefixRemoved.Length - suffix.Length - 1);

            if (ushort.TryParse(numberPart, out _))
            {
                yield return file;
            }
        }
    }
}