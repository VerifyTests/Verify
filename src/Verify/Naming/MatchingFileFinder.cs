static class MatchingFileFinder
{
    public static IEnumerable<string> Find(List<string> files, string fileNamePrefix, string suffix)
    {
        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (!name.StartsWith(fileNamePrefix))
            {
                continue;
            }

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

            if (int.TryParse(numberPart, out _))
            {
                yield return file;
            }
        }
    }
}