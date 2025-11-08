public static class StringBuilderExtensions
{
    public static void ReplaceDirectoryPaths(this StringBuilder builder, List<KeyValuePair<string, string>> paths)
    {
        if (builder.Length == 0 || paths.Count == 0)
            return;

        // Copy StringBuilder content using GetChunks() to avoid string allocation
        var buffer = ArrayPool<char>.Shared.Rent(builder.Length);
        try
        {
            var position = 0;
            foreach (var chunk in builder.GetChunks())
            {
                chunk.Span.CopyTo(buffer.AsSpan(position));
                position += chunk.Length;
            }

            var content = buffer.AsSpan(0, builder.Length);

            // Find all matches for all paths
            var replacements = new List<Replacement>();

            foreach (var kvp in paths)
            {
                FindMatches(content, kvp.Key, kvp.Value, replacements);
            }

            // Remove overlapping matches, keeping the longest one
            replacements = RemoveOverlaps(replacements);

            // Sort by position descending to maintain indices during replacement
            replacements.Sort((a, b) => b.Index.CompareTo(a.Index));

            // Apply replacements
            foreach (var replacement in replacements)
            {
                builder.Remove(replacement.Index, replacement.Length);
                builder.Insert(replacement.Index, replacement.Value);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static List<Replacement> RemoveOverlaps(List<Replacement> replacements)
    {
        if (replacements.Count <= 1)
            return replacements;

        // Sort by index, then by length descending (prefer longer matches)
        replacements.Sort((a, b) =>
        {
            var indexCompare = a.Index.CompareTo(b.Index);
            if (indexCompare != 0)
                return indexCompare;
            return b.Length.CompareTo(a.Length);
        });

        var result = new List<Replacement>();
        var lastEnd = -1;

        foreach (var replacement in replacements)
        {
            // If this replacement doesn't overlap with the last one, keep it
            if (replacement.Index >= lastEnd)
            {
                result.Add(replacement);
                lastEnd = replacement.Index + replacement.Length;
            }
        }

        return result;
    }

    private static void FindMatches(
        ReadOnlySpan<char> content,
        string find,
        string replace,
        List<Replacement> replacements)
    {
        var searchStart = 0;

        while (searchStart <= content.Length - find.Length)
        {
            var matchIndex = FindNextMatch(content, find, searchStart);
            if (matchIndex == -1)
                break;

            // Validate preceding character
            if (matchIndex > 0)
            {
                var preceding = content[matchIndex - 1];
                if (char.IsLetterOrDigit(preceding))
                {
                    searchStart = matchIndex + 1;
                    continue;
                }
            }

            // Check trailing character and determine match length
            var matchLength = find.Length;
            var trailingIndex = matchIndex + find.Length;

            if (trailingIndex < content.Length)
            {
                var trailing = content[trailingIndex];

                // Invalid if trailing is letter or digit
                if (char.IsLetterOrDigit(trailing))
                {
                    searchStart = matchIndex + 1;
                    continue;
                }

                // Greedy: include trailing separator
                if (trailing == '/' || trailing == '\\')
                {
                    matchLength++;
                }
            }

            replacements.Add(new Replacement(matchIndex, matchLength, replace));
            searchStart = matchIndex + find.Length;
        }
    }

    private static int FindNextMatch(ReadOnlySpan<char> content, string find, int startIndex)
    {
        for (var i = startIndex; i <= content.Length - find.Length; i++)
        {
            if (IsPathMatch(content.Slice(i, find.Length), find))
            {
                return i;
            }
        }
        return -1;
    }

    private static bool IsPathMatch(ReadOnlySpan<char> span, string find)
    {
        if (span.Length != find.Length)
            return false;

        for (var i = 0; i < span.Length; i++)
        {
            var c1 = span[i];
            var c2 = find[i];

            // Treat / and \ as equivalent
            if (c1 == '/' || c1 == '\\')
            {
                if (c2 != '/' && c2 != '\\')
                    return false;
            }
            else if (c1 != c2)
            {
                return false;
            }
        }

        return true;
    }

    private readonly struct Replacement
    {
        public readonly int Index;
        public readonly int Length;
        public readonly string Value;

        public Replacement(int index, int length, string value)
        {
            Index = index;
            Length = length;
            Value = value;
        }
    }
}