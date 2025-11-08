public static class StringBuilderExtensions
{
    public static void ReplaceDirectoryPaths(this StringBuilder builder, List<KeyValuePair<string, string>> paths)
    {
        if (builder.Length == 0 || paths.Count == 0)
            return;

        var replacements = new List<Replacement>();

        // Find all matches
        foreach (var kvp in paths)
        {
            FindMatches(builder, kvp.Key, kvp.Value, replacements);
        }

        // Remove overlaps
        replacements = RemoveOverlaps(replacements);

        // Sort by position descending
        replacements.Sort((a, b) => b.Index.CompareTo(a.Index));

        // Apply replacements
        foreach (var replacement in replacements)
        {
            builder.Remove(replacement.Index, replacement.Length);
            builder.Insert(replacement.Index, replacement.Value);
        }
    }

    private static void FindMatches(
        StringBuilder builder,
        string find,
        string replace,
        List<Replacement> replacements)
    {
        var position = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var span = chunk.Span;

            for (var i = 0; i < span.Length; i++)
            {
                var absolutePos = position + i;

                // Check if we have enough characters left
                if (absolutePos + find.Length > builder.Length)
                    break;

                // Try to match at this position
                if (TryMatchAt(builder, absolutePos, find, out var matchLength))
                {
                    replacements.Add(new Replacement(absolutePos, matchLength, replace));
                }
            }

            position += span.Length;
        }
    }

    private static bool TryMatchAt(
        StringBuilder builder,
        int absolutePos,
        string find,
        out int matchLength)
    {
        matchLength = 0;

        // Check preceding character
        if (absolutePos > 0)
        {
            var preceding = GetCharAt(builder, absolutePos - 1);
            if (char.IsLetterOrDigit(preceding))
                return false;
        }

        // Check if the path matches
        if (!IsPathMatchAt(builder, absolutePos, find))
            return false;

        // Check trailing character
        matchLength = find.Length;
        var trailingPos = absolutePos + find.Length;

        if (trailingPos < builder.Length)
        {
            var trailing = GetCharAt(builder, trailingPos);

            // Invalid if trailing is letter or digit
            if (char.IsLetterOrDigit(trailing))
                return false;

            // Greedy: include trailing separator
            if (trailing == '/' || trailing == '\\')
                matchLength++;
        }

        return true;
    }

    private static bool IsPathMatchAt(StringBuilder builder, int absolutePos, string find)
    {
        var findIndex = 0;
        var currentPos = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var span = chunk.Span;

            // Skip chunks before our start position
            if (currentPos + span.Length <= absolutePos)
            {
                currentPos += span.Length;
                continue;
            }

            // Determine where to start in this chunk
            var startInChunk = Math.Max(0, absolutePos - currentPos);

            // Match characters in this chunk
            for (var i = startInChunk; i < span.Length && findIndex < find.Length; i++)
            {
                var c1 = span[i];
                var c2 = find[findIndex];

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

                findIndex++;
            }

            // If we've matched everything, we're done
            if (findIndex == find.Length)
                return true;

            currentPos += span.Length;
        }

        return false;
    }

    private static char GetCharAt(StringBuilder builder, int absolutePos)
    {
        var currentPos = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var span = chunk.Span;
            if (absolutePos < currentPos + span.Length)
            {
                return span[absolutePos - currentPos];
            }
            currentPos += span.Length;
        }

        throw new ArgumentOutOfRangeException(nameof(absolutePos));
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