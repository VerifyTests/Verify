public static class StringBuilderExtensions
{
    public static void ReplaceDirectoryPaths(
        this StringBuilder builder,
        List<KeyValuePair<string, string>> paths)
    {
        if (builder.Length == 0 || paths.Count == 0)
        {
            return;
        }

        // Find all matches with their positions
        var replacements = new List<Replacement>();

        foreach (var path in paths)
        {
            if (string.IsNullOrEmpty(path.Key))
            {
                continue;
            }

            FindMatches(builder, path.Key, path.Value, replacements);
        }

        if (replacements.Count == 0)
        {
            return;
        }

        // Sort by position descending to avoid index shifts during replacement
        replacements.Sort((a, b) => b.Position.CompareTo(a.Position));

        // Remove overlapping replacements (keep first found, which is last by position)
        var filtered = new List<Replacement>();
        var lastEnd = int.MaxValue;

        foreach (var replacement in replacements)
        {
            var replacementEnd = replacement.Position + replacement.Length;
            if (replacementEnd <= lastEnd)
            {
                filtered.Add(replacement);
                lastEnd = replacement.Position;
            }
        }

        // Apply replacements in reverse order
        foreach (var replacement in filtered)
        {
            builder.Remove(replacement.Position, replacement.Length);
            builder.Insert(replacement.Position, replacement.Value);
        }
    }

    private static void FindMatches(
        StringBuilder builder,
        string searchKey,
        string replaceValue,
        List<Replacement> replacements)
    {
        var keySpan = searchKey.AsSpan();
        var position = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;
            var chunkOffset = 0;

            while (chunkOffset <= chunkSpan.Length - keySpan.Length)
            {
                var remaining = chunkSpan[chunkOffset..];
                var matchIndex = FindPotentialMatch(remaining, keySpan);

                if (matchIndex == -1)
                {
                    break;
                }

                var absolutePosition = position + chunkOffset + matchIndex;

                // Verify match and check word boundaries
                if (IsCompleteMatch(builder, absolutePosition, keySpan) &&
                    HasValidBoundaries(builder, absolutePosition, keySpan.Length))
                {
                    // Check for trailing directory separator and include it greedily
                    var matchLength = keySpan.Length;
                    var endPosition = absolutePosition + matchLength;

                    if (endPosition < builder.Length)
                    {
                        var nextChar = builder[endPosition];
                        if (nextChar == '/' || nextChar == '\\')
                        {
                            matchLength++;
                            // Characters following a directory separator are always valid
                            // so no additional boundary check needed
                        }
                    }

                    replacements.Add(new Replacement(
                        absolutePosition,
                        matchLength,
                        replaceValue));
                }

                chunkOffset += matchIndex + 1;
            }

            position += chunk.Length;
        }
    }

    private static int FindPotentialMatch(ReadOnlySpan<char> span, ReadOnlySpan<char> searchKey)
    {
        if (searchKey.Length == 0)
        {
            return -1;
        }

        var firstChar = searchKey[0];
        var firstCharAlt = GetDirectorySeparatorAlternative(firstChar);

        for (var i = 0; i <= span.Length - searchKey.Length; i++)
        {
            if (span[i] == firstChar || span[i] == firstCharAlt)
            {
                var matches = true;
                for (var j = 1; j < searchKey.Length; j++)
                {
                    if (!AreCharsEqual(span[i + j], searchKey[j]))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private static bool IsCompleteMatch(
        StringBuilder builder,
        int position,
        ReadOnlySpan<char> searchKey)
    {
        // Verify the match using direct indexing
        // (handles case where match might span chunk boundaries)
        if (position + searchKey.Length > builder.Length)
        {
            return false;
        }

        for (var i = 0; i < searchKey.Length; i++)
        {
            if (!AreCharsEqual(builder[position + i], searchKey[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreCharsEqual(char c1, char c2)
    {
        if (c1 == c2)
        {
            return true;
        }

        // Treat / and \ as equivalent
        if ((c1 == '/' || c1 == '\\') && (c2 == '/' || c2 == '\\'))
        {
            return true;
        }

        return false;
    }

    private static char GetDirectorySeparatorAlternative(char c)
    {
        if (c == '/')
        {
            return '\\';
        }

        if (c == '\\')
        {
            return '/';
        }

        return c;
    }

    private static bool HasValidBoundaries(
        StringBuilder builder,
        int position,
        int length)
    {
        // Check previous character - must not be letter or digit
        if (position > 0)
        {
            var prevChar = builder[position - 1];
            if (char.IsLetterOrDigit(prevChar))
            {
                return false;
            }
        }

        // Check trailing character - must not be letter or digit
        // UNLESS it's a directory separator (which will be included greedily)
        var endPosition = position + length;
        if (endPosition < builder.Length)
        {
            var nextChar = builder[endPosition];

            // Directory separators are valid boundaries (and will be included greedily)
            // Any character following a separator is implicitly valid
            if (nextChar is '/' or '\\')
            {
                return true;
            }

            // Otherwise, must not be alphanumeric
            if (char.IsLetterOrDigit(nextChar))
            {
                return false;
            }
        }

        return true;
    }

    private readonly struct Replacement(int position, int length, string value)
    {
        public int Position { get; } = position;
        public int Length { get; } = length;
        public string Value { get; } = value;
    }
}