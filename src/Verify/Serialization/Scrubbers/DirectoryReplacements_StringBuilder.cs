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

    static List<Match> FindMatches(StringBuilder builder, List<Pair> pairs)
    {
        if (pairs.Count == 0)
        {
            return [];
        }

        var matches = new List<Match>();
        // Track matched positions
        var matchedRanges = new List<(int Start, int End)>();
        var absolutePosition = 0;

        // Find the longest path to determine buffer sizes
        var maxPathLength = pairs.Max(p => p.Find.Length);
        var carryoverSize = maxPathLength - 1;

        Span<char> carryoverBuffer = stackalloc char[carryoverSize];
        Span<char> combinedBuffer = stackalloc char[maxPathLength * 2];
        var carryoverLength = 0;
        var previousChunkAbsoluteEnd = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;

            // Check for matches spanning from previous chunk to current chunk
            if (carryoverLength > 0)
            {
                for (var carryoverIndex = 0; carryoverIndex < carryoverLength; carryoverIndex++)
                {
                    foreach (var pair in pairs)
                    {
                        var remainingInCarryover = carryoverLength - carryoverIndex;
                        var neededFromCurrent = pair.Find.Length - remainingInCarryover;

                        if (neededFromCurrent <= 0 || neededFromCurrent > chunkSpan.Length)
                        {
                            continue;
                        }

                        var combinedLength = remainingInCarryover + neededFromCurrent;
                        carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(combinedBuffer);
                        chunkSpan.Slice(0, neededFromCurrent).CopyTo(combinedBuffer.Slice(remainingInCarryover));

                        var startPosition = previousChunkAbsoluteEnd - carryoverLength + carryoverIndex;

                        // Check if this position overlaps with existing match
                        if (OverlapsExistingMatch(startPosition, pair.Find.Length, matchedRanges))
                        {
                            continue;
                        }

                        if (TryMatchAtCrossChunk(
                                builder,
                                combinedBuffer.Slice(0, combinedLength),
                                chunkSpan,
                                startPosition,
                                neededFromCurrent,
                                pair.Find,
                                out var matchLength))
                        {
                            matches.Add(new(startPosition, matchLength, pair.Replace));
                            matchedRanges.Add((startPosition, startPosition + matchLength));
                            // Found a match at this position, skip other pairs
                            break;
                        }
                    }
                }
            }

            // Process matches entirely within this chunk
            for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
            {
                var absoluteIndex = absolutePosition + chunkIndex;

                // Skip if already matched
                if (IsPositionMatched(absoluteIndex, matchedRanges))
                {
                    continue;
                }

                foreach (var pair in pairs)
                {
                    // Check if we have enough characters left in this chunk
                    if (chunkIndex + pair.Find.Length > chunk.Length)
                    {
                        continue;
                    }

                    // Check if this would overlap with existing match
                    if (OverlapsExistingMatch(absoluteIndex, pair.Find.Length, matchedRanges))
                    {
                        continue;
                    }

                    // Try to match at this position
                    if (TryMatchAt(chunk, chunkIndex, pair.Find, out var matchLength))
                    {
                        matches.Add(new(absoluteIndex, matchLength, pair.Replace));
                        matchedRanges.Add((absoluteIndex, absoluteIndex + matchLength));
                        // Skip past this match
                        chunkIndex += matchLength - 1;
                        // Found a match, skip other pairs at this position
                        break;
                    }
                }
            }

            // Save last N chars for next iteration
            carryoverLength = Math.Min(carryoverSize, chunk.Length);
            chunkSpan.Slice(chunk.Length - carryoverLength, carryoverLength).CopyTo(carryoverBuffer);

            previousChunkAbsoluteEnd = absolutePosition + chunk.Length;
            absolutePosition += chunk.Length;
        }

        return matches;
    }

    static bool IsPositionMatched(int position, List<(int Start, int End)> matchedRanges)
    {
        foreach (var range in matchedRanges)
        {
            if (position >= range.Start && position < range.End)
            {
                return true;
            }
        }

        return false;
    }

    static bool OverlapsExistingMatch(int start, int length, List<(int Start, int End)> matchedRanges)
    {
        var end = start + length;
        foreach (var range in matchedRanges)
        {
            // Check if ranges overlap
            if (start < range.End && end > range.Start)
            {
                return true;
            }
        }

        return false;
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

    readonly struct Match(int index, int length, string value)
    {
        public readonly int Index = index;
        public readonly int Length = length;
        public readonly string Value = value;
    }
}