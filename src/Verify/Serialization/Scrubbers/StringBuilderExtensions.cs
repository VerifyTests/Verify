public static class StringBuilderExtensions
{
    public static void ReplaceDirectoryPaths(
        this StringBuilder builder,
        List<KeyValuePair<string, string>> paths)
    {
        if (builder.Length == 0)
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
            if (replacementEnd > lastEnd)
            {
                continue;
            }

            filtered.Add(replacement);
            lastEnd = replacement.Position;
        }

        // Apply replacements in reverse order
        foreach (var replacement in filtered)
        {
            builder.Remove(replacement.Position, replacement.Length);
            builder.Insert(replacement.Position, replacement.Value);
        }
    }

    static void FindMatches(
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
                var matchIndex = remaining.IndexOf(keySpan, StringComparison.Ordinal);

                if (matchIndex == -1)
                {
                    break;
                }

                var absolutePosition = position + chunkOffset + matchIndex;

                // Verify match and check word boundaries
                if (IsCompleteMatch(builder, absolutePosition, keySpan) &&
                    HasValidBoundaries(builder, absolutePosition, keySpan.Length))
                {
                    replacements.Add(new Replacement(
                        absolutePosition,
                        keySpan.Length,
                        replaceValue));
                }

                chunkOffset += matchIndex + 1;
            }

            position += chunk.Length;
        }
    }

    static bool IsCompleteMatch(
        StringBuilder builder,
        int position,
        CharSpan searchKey)
    {
        // Verify the match using direct indexing
        // (handles case where match might span chunk boundaries)
        if (position + searchKey.Length > builder.Length)
        {
            return false;
        }

        for (var i = 0; i < searchKey.Length; i++)
        {
            if (builder[position + i] != searchKey[i])
            {
                return false;
            }
        }

        return true;
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
        var endPosition = position + length;
        if (endPosition < builder.Length)
        {
            var nextChar = builder[endPosition];
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