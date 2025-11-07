public static class StringBuilderExtensions
{
    private static readonly bool IsWindows = OperatingSystem.IsWindows();

    public static void ReplaceDirectoryPaths(this StringBuilder builder, List<KeyValuePair<string, string>> paths)
    {
        foreach (var (searchPath, replacement) in paths)
        {
            // Normalize search path to use forward slashes
            var normalizedSearch = searchPath.Replace('\\', '/');

            // Find all matches using GetChunks
            var matches = FindDirectoryPathMatches(builder, normalizedSearch);

            // Replace from end to start to keep indices valid
            foreach (var (startIndex, length) in matches.OrderByDescending(m => m.startIndex))
            {
                builder.Remove(startIndex, length);
                builder.Insert(startIndex, replacement);
            }
        }
    }

    private static List<(int startIndex, int length)> FindDirectoryPathMatches(StringBuilder builder, string normalizedSearch)
    {
        var matches = new List<(int, int)>();
        var chunkStartPosition = 0;

        // Use GetChunks to search through the StringBuilder efficiently
        foreach (var chunk in builder.GetChunks())
        {
            var span = chunk.Span;

            // Search for potential matches in this chunk
            for (var i = 0; i < span.Length; i++)
            {
                var absolutePosition = chunkStartPosition + i;

                // Check if a match could start at this position
                if (IsMatchAt(builder, absolutePosition, normalizedSearch, out var matchLength))
                {
                    matches.Add((absolutePosition, matchLength));
                }
            }

            chunkStartPosition += span.Length;
        }

        return matches;
    }

    private static bool IsMatchAt(StringBuilder builder, int startIndex, string normalizedSearch, out int matchLength)
    {
        matchLength = 0;

        // Check if we have enough characters remaining
        if (startIndex + normalizedSearch.Length > builder.Length)
            return false;

        // Match the search path (normalizing directory separators)
        for (var i = 0; i < normalizedSearch.Length; i++)
        {
            var builderChar = builder[startIndex + i];
            var normalizedBuilderChar = builderChar == '\\' ? '/' : builderChar;

            if (normalizedBuilderChar != normalizedSearch[i])
                return false;
        }

        // Validate preceding character
        if (startIndex > 0)
        {
            var precedingChar = builder[startIndex - 1];
            if (IsInvalidPrecedingChar(precedingChar))
                return false;
        }

        // Validate and determine trailing character behavior
        var endIndex = startIndex + normalizedSearch.Length;
        if (endIndex < builder.Length)
        {
            var trailingChar = builder[endIndex];

            // Invalid if followed by letter or digit
            if (IsInvalidTrailingChar(trailingChar))
                return false;

            // Greedy: include directory separator in the match
            if (IsDirectorySeparator(trailingChar))
            {
                matchLength = normalizedSearch.Length + 1;
                return true;
            }
        }

        matchLength = normalizedSearch.Length;
        return true;
    }

    private static bool IsInvalidPrecedingChar(char c)
    {
        // Letters and digits are always invalid
        if (char.IsLetterOrDigit(c))
            return true;

        // Directory separator is invalid on Linux/Mac, valid on Windows
        if (IsDirectorySeparator(c))
            return !IsWindows;

        return false;
    }

    private static bool IsInvalidTrailingChar(char c) =>
        char.IsLetterOrDigit(c);

    private static bool IsDirectorySeparator(char c) =>
        c == '/' || c == '\\';
}