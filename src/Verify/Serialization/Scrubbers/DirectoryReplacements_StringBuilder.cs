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
        if (builder.Length == 0)
        {
            return;
        }

        var matches = FindMatches(builder, paths);

        // Sort by position descending
        var orderByDescending = matches.OrderByDescending(_ => _.Index);

        // Apply matches
        foreach (var match in orderByDescending)
        {
            builder.Overwrite(match.Value, match.Index, match.Length);
        }
    }

    static IEnumerable<Match> FindMatches(StringBuilder builder, List<Pair> pairs)
    {
        var absolutePosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
            {
                foreach (var pair in pairs)
                {
                    // Check if we have enough characters left in this chunk
                    if (chunkIndex + pair.Find.Length > chunk.Length)
                    {
                        continue;
                    }

                    // Try to match at this position
                    if (TryMatchAt(chunk, chunkIndex, pair.Find, out var matchLength))
                    {
                        var startReplaceIndex = absolutePosition + chunkIndex;
                        yield return new(startReplaceIndex, matchLength, pair.Replace);
                        chunkIndex += pair.Find.Length;
                    }
                }
            }

            absolutePosition += chunk.Length;
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
            var chunkChar = chunk[chunkPos + i];
            var findChar = find[i];

            // Treat / and \ as equivalent
            if (chunkChar is '/' or '\\')
            {
                if (findChar != '/')
                {
                    return false;
                }

                continue;
            }

            if (chunkChar != findChar)
            {
                return false;
            }
        }

        return true;
    }

    readonly struct Match(int index, int length, string value)
    {
        public readonly int Index = index;
        public readonly int Length = length;
        public readonly string Value = value;
    }
}