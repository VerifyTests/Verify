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
        var context = new MatchContext(paths);

        CrossChunkMatcher.ReplaceAll<MatchContext>(
            builder,
            carryoverSize: maxLength - 1,
            context,
            OnCrossChunk,
            OnWithinChunk);
    }

    static void OnCrossChunk(
        StringBuilder builder,
        Span<char> carryoverBuffer,
        int carryoverIndex,
        int remainingInCarryover,
        CharSpan currentChunkSpan,
        int absoluteStartPosition,
        MatchContext context,
        Action<Match> addMatch)
    {
        Span<char> combinedBuffer = stackalloc char[context.MaxLength * 2];

        foreach (var pair in context.Pairs)
        {
            var neededFromCurrent = pair.Find.Length - remainingInCarryover;

            if (neededFromCurrent <= 0 ||
                neededFromCurrent > currentChunkSpan.Length)
            {
                continue;
            }

            // Check if this position overlaps with existing match
            if (context.OverlapsExistingMatch(absoluteStartPosition, pair.Find.Length))
            {
                continue;
            }

            var combinedLength = remainingInCarryover + neededFromCurrent;
            carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(combinedBuffer);
            currentChunkSpan[..neededFromCurrent].CopyTo(combinedBuffer[remainingInCarryover..]);

            if (!TryMatchAtCrossChunk(
                    builder,
                    combinedBuffer[..combinedLength],
                    currentChunkSpan,
                    absoluteStartPosition,
                    neededFromCurrent,
                    pair.Find,
                    out var matchLength))
            {
                continue;
            }

            addMatch(new(absoluteStartPosition, matchLength, pair.Replace));
            context.AddMatchedRange(absoluteStartPosition, absoluteStartPosition + matchLength);
            // Found a match at this position, skip other pairs
            break;
        }
    }

    static int OnWithinChunk(
        ReadOnlyMemory<char> chunk,
        CharSpan chunkSpan,
        int chunkIndex,
        int absoluteIndex,
        MatchContext context,
        Action<Match> addMatch)
    {
        // Skip if already matched
        if (context.IsPositionMatched(absoluteIndex))
        {
            return 1;
        }

        foreach (var pair in context.Pairs)
        {
            // Check if we have enough characters left in this chunk
            if (chunkIndex + pair.Find.Length > chunk.Length)
            {
                continue;
            }

            // Check if this would overlap with existing match
            if (context.OverlapsExistingMatch(absoluteIndex, pair.Find.Length))
            {
                continue;
            }

            // Try to match at this position
            if (!TryMatchAt(chunk, chunkIndex, pair.Find, out var matchLength))
            {
                continue;
            }

            addMatch(new(absoluteIndex, matchLength, pair.Replace));
            context.AddMatchedRange(absoluteIndex, absoluteIndex + matchLength);
            // Skip past this match
            return matchLength;
        }

        return 1;
    }

    static bool TryMatchAtCrossChunk(
        StringBuilder builder,
        CharSpan combinedSpan,
        CharSpan currentChunkSpan,
        int absoluteStartPosition,
        int neededFromCurrent,
        string find,
        out int matchLength)
    {
        matchLength = 0;

        // Check preceding character
        if (absoluteStartPosition > 0)
        {
            var preceding = builder[absoluteStartPosition - 1];
            if (char.IsLetterOrDigit(preceding))
            {
                return false;
            }
        }

        // Check if the path matches
        if (!IsPathMatchAt(combinedSpan, 0, find))
        {
            return false;
        }

        matchLength = find.Length;

        // Check trailing character (it's in the current chunk)
        if (neededFromCurrent < currentChunkSpan.Length)
        {
            var trailing = currentChunkSpan[neededFromCurrent];

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
        }

        return true;
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

    sealed class MatchContext(List<Pair> pairs)
    {
        public List<Pair> Pairs { get; } = pairs;
        public int MaxLength { get; } = pairs.Count > 0 ? pairs[0].Find.Length : 0;

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
            foreach (var range in matchedRanges)
            {
                if (start < range.End && end > range.Start)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
