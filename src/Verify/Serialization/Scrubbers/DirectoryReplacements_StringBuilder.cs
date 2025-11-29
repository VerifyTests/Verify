static partial class DirectoryReplacements
{
    public readonly struct Pair
    {
        public Pair(string find, string replace)
        {
#if DEBUG
            if (find.Contains('\\'))
            {
                throw new("Slashes should be sanitized");
            }
#endif
            Find = find;
            Replace = replace;
        }

        public string Find { get; }
        public string Replace { get; }
    }

    public static void Replace(StringBuilder builder, List<Pair> paths)
    {
#if DEBUG
        var finds = paths.Select(_ => _.Find).ToList();
        if (!finds.OrderByDescending(_ => _.Length).SequenceEqual(finds))
        {
            throw new("Pairs should be ordered");
        }

        if (finds.Count != finds.Distinct().Count())
        {
            throw new("Find should be distinct");
        }
#endif
        if (builder.Length == 0 || paths.Count == 0)
        {
            return;
        }

        // pairs are ordered by length, so max length is the first one
        var maxLength = paths[0].Find.Length;
        var context = new MatchContext(builder, paths);

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: maxLength,
            context,
            matcher: static (content, absolutePosition, context) =>
            {
                // Skip if already matched
                if (context.IsPositionMatched(absolutePosition))
                {
                    return MatchResult.NoMatch();
                }

                foreach (var pair in context.Pairs)
                {
                    // Not enough content for this pattern
                    if (content.Length < pair.Find.Length)
                    {
                        continue;
                    }

                    // Check if this would overlap with existing match
                    if (context.OverlapsExistingMatch(absolutePosition, pair.Find.Length))
                    {
                        continue;
                    }

                    // Try to match at this position
                    if (!TryMatchAt(context.Builder, content, absolutePosition, pair.Find, out var matchLength))
                    {
                        continue;
                    }

                    context.AddMatchedRange(absolutePosition, absolutePosition + matchLength);
                    return MatchResult.Match(matchLength, pair.Replace);
                }

                return MatchResult.NoMatch();
            });
    }

    static bool TryMatchAt(
        StringBuilder builder,
        CharSpan content,
        int absolutePosition,
        string find,
        out int matchLength)
    {
        matchLength = 0;

        // Check preceding character
        if (absolutePosition > 0)
        {
            var preceding = builder[absolutePosition - 1];
            if (char.IsLetterOrDigit(preceding))
            {
                return false;
            }
        }

        // Check if the path matches
        if (!IsPathMatchAt(content, 0, find))
        {
            return false;
        }

        matchLength = find.Length;

        // Check trailing character
        var trailingPosition = absolutePosition + find.Length;
        if (trailingPosition >= builder.Length)
        {
            return true;
        }

        // Check if we have the trailing character in our content window
        // or need to look it up in the builder
        char trailing;
        if (find.Length < content.Length)
        {
            // Trailing char is in our window
            trailing = content[find.Length];
        }
        else
        {
            // Need to look up in builder
            trailing = builder[trailingPosition];
        }

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

    static bool IsPathMatchAt(CharSpan content, int contentPos, string find)
    {
        for (var i = 0; i < find.Length; i++)
        {
            var contentChar = content[contentPos + i];
            var findChar = find[i];

            // Treat / and \ as equivalent
            if (contentChar is '/' or '\\')
            {
                if (findChar != '/')
                {
                    return false;
                }

                continue;
            }

            if (contentChar != findChar)
            {
                return false;
            }
        }

        return true;
    }

    sealed class MatchContext(StringBuilder builder, List<Pair> pairs)
    {
        public StringBuilder Builder { get; } = builder;
        public List<Pair> Pairs { get; } = pairs;
        List<(int Start, int End)> matchedRanges = [];

        public void AddMatchedRange(int start, int end) =>
            matchedRanges.Add((start, end));

        public bool IsPositionMatched(int position)
        {
            foreach (var (start, end) in matchedRanges)
            {
                if (position >= start && position < end)
                {
                    return true;
                }
            }

            return false;
        }

        public bool OverlapsExistingMatch(int start, int length)
        {
            var end = start + length;
            return matchedRanges.Any(range => start < range.End && end > range.Start);
        }
    }
}