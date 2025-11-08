static partial class DirectoryReplacements
{
    public readonly struct Pair(string find, string replace)
    {
        public string Find { get; } = find;
        public string Replace { get; } = replace;
    }

    public static void Replace(StringBuilder builder, List<Pair> paths)
    {
        if (builder.Length == 0 || paths.Count == 0)
        {
            return;
        }

        var matches = new List<Match>();

        foreach (var pair in paths)
        {
            FindMatches(builder, pair, matches);
        }

        matches = RemoveOverlaps(matches);

        // Sort by position descending
        matches.Sort((a, b) => b.Index.CompareTo(a.Index));

        // Apply matches
        foreach (var match in matches)
        {
            builder.Remove(match.Index, match.Length);
            builder.Insert(match.Index, match.Value);
        }
    }

    static void FindMatches(StringBuilder builder, Pair pair, List<Match> matches)
    {
        var position = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var span = chunk.Span;

            for (var i = 0; i < span.Length; i++)
            {
                var absolutePosition = position + i;

                // Check if we have enough characters left
                if (absolutePosition + pair.Find.Length > builder.Length)
                {
                    break;
                }

                // Try to match at this position
                if (TryMatchAt(builder, absolutePosition, pair.Find, out var matchLength))
                {
                    matches.Add(new(absolutePosition, matchLength, pair.Replace));
                }
            }

            position += span.Length;
        }
    }

    static bool TryMatchAt(StringBuilder builder, int absolutePos, string find, out int matchLength)
    {
        matchLength = 0;

        // Check preceding character
        if (absolutePos > 0)
        {
            var preceding = builder[absolutePos - 1];
            if (char.IsLetterOrDigit(preceding))
            {
                return false;
            }
        }

        // Check if the path matches
        if (!IsPathMatchAt(builder, absolutePos, find))
        {
            return false;
        }

        // Check trailing character
        matchLength = find.Length;
        var trailingPos = absolutePos + find.Length;

        if (trailingPos >= builder.Length)
        {
            return true;
        }

        var trailing = builder[trailingPos];

        // Invalid if trailing is letter or digit
        if (char.IsLetterOrDigit(trailing))
        {
            return false;
        }

        // Greedy: include trailing separator
        if (trailing is '/' or '\\')
        {
            matchLength++;
        }

        return true;
    }

    static bool IsPathMatchAt(StringBuilder builder, int absolutePos, string find)
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
                var currentCh = span[i];
                var findCh = find[findIndex];

                // Treat / and \ as equivalent
                if (currentCh is '/' or '\\')
                {
                    if (findCh != '/')
                    {
                        return false;
                    }
                }
                else if (currentCh != findCh)
                {
                    return false;
                }

                findIndex++;
            }

            // If we've matched everything, we're done
            if (findIndex == find.Length)
            {
                return true;
            }

            currentPos += span.Length;
        }

        return false;
    }

    static List<Match> RemoveOverlaps(List<Match> matches)
    {
        if (matches.Count <= 1)
        {
            return matches;
        }

        // Sort by index, then by length descending (prefer longer matches)
        matches.Sort((a, b) =>
        {
            var indexCompare = a.Index.CompareTo(b.Index);
            if (indexCompare != 0)
            {
                return indexCompare;
            }

            return b.Length.CompareTo(a.Length);
        });

        var result = new List<Match>();
        var lastEnd = -1;

        foreach (var match in matches)
        {
            // If this match doesn't overlap with the last one, keep it
            if (match.Index >= lastEnd)
            {
                result.Add(match);
                lastEnd = match.Index + match.Length;
            }
        }

        return result;
    }

    readonly struct Match(int index, int length, string value)
    {
        public readonly int Index = index;
        public readonly int Length = length;
        public readonly string Value = value;
    }
}