//Assumption is that directories wonr span chunks
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
        var finds = paths.Select(_=>_.Find).ToList();
        if (!finds.OrderByDescending(_ => _).SequenceEqual(finds))
        {
            throw new("Pairs should be ordered");
        }
#endif
        if (builder.Length == 0)
        {
            return;
        }

        var matches = FindMatches(builder, paths);

        var removeOverlaps = RemoveOverlaps(matches);

        // Sort by position descending
        var orderByDescending = removeOverlaps.OrderByDescending(_ => _.Index);

        // Apply matches
        foreach (var match in orderByDescending)
        {
            builder.Remove(match.Index, match.Length);
            builder.Insert(match.Index, match.Value);
        }
    }

    static IEnumerable<Match> FindMatches(StringBuilder builder, List<Pair> pairs)
    {

        foreach (var pair in pairs)
        {
            var position = 0;

            foreach (var chunk in builder.GetChunks())
            {
                for (var i = 0; i < chunk.Length; i++)
                {
                    // Check if we have enough characters left in this chunk
                    if (i + pair.Find.Length > chunk.Length)
                    {
                        break;
                    }

                    var absolutePosition = position + i;

                    // Try to match at this position
                    if (TryMatchAt(chunk, i, pair.Find, out var matchLength))
                    {
                        yield return new(absolutePosition, matchLength, pair.Replace);
                    }
                }

                position += chunk.Length;
            }
        }
    }

    static bool TryMatchAt(ReadOnlyMemory<char> chunk, int chunkPos, string find, out int matchLength)
    {
        var span = chunk.Span;
        matchLength = 0;

        // Check preceding character
        if (chunkPos > 0)
        {
            var preceding = span[chunkPos - 1];
            if (char.IsLetterOrDigit(preceding))
            {
                return false;
            }
        }

        // Check if the path matches
        if (!IsPathMatchAt(span, chunkPos, find))
        {
            return false;
        }

        // Check trailing character
        matchLength = find.Length;
        var trailingPos = chunkPos + find.Length;

        if (trailingPos >= span.Length)
        {
            return true;
        }

        var trailing = span[trailingPos];

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

    static bool IsPathMatchAt(CharSpan chunk, int chunkPos, string find)
    {
        for (var i = 0; i < find.Length; i++)
        {
            var currentCh = chunk[chunkPos + i];
            var findCh = find[i];

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
        }

        return true;
    }

    static IEnumerable<Match> RemoveOverlaps(IEnumerable<Match> matches)
    {
        var lastEnd = -1;

        foreach (var match in matches
                     .OrderBy(_ => _.Index)
                     .ThenByDescending(_ => _.Length))
        {
            // If this match overlaps with the last one, discard it
            if (match.Index < lastEnd)
            {
                continue;
            }

            yield return match;
            lastEnd = match.Index + match.Length;
        }
    }

    readonly struct Match(int index, int length, string value)
    {
        public readonly int Index = index;
        public readonly int Length = length;
        public readonly string Value = value;
    }
}