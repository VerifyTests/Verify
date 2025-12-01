/// <summary>
/// Helper for matching and replacing patterns in StringBuilder that may span across chunk boundaries.
/// </summary>
static class CrossChunkMatcher
{
    /// <summary>
    /// Finds all matches in a StringBuilder (handling patterns spanning chunk boundaries) and applies replacements.
    /// </summary>
    /// <param name="builder">The StringBuilder to search and modify</param>
    /// <param name="maxLength">Maximum pattern length to search for</param>
    /// <param name="context">User context passed to callbacks</param>
    /// <param name="matcher">Called for each potential match position with accumulated buffer</param>
    public static void ReplaceAll<TContext>(
        StringBuilder builder,
        int maxLength,
        TContext context,
        MatchHandler<TContext> matcher)
    {
        if (maxLength <= 0)
        {
            throw new ArgumentException("maxLength must be positive", nameof(maxLength));
        }

        // Fast path for single chunk
        if (builder.TryGetSingleChunk(out var chunk))
        {
            // Only one chunk - use optimized path
            ReplaceAllSingleChunk(builder, chunk.Span, maxLength, context, matcher);
            return;
        }

        // Multi-chunk path
        ReplaceAllMultiChunk(builder, maxLength, context, matcher);
    }
#if NET8_0_OR_GREATER
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "m_ChunkPrevious")]
    static extern ref StringBuilder? GetChunkPrevious(StringBuilder builder);

    static bool HasMultipleChunks(this StringBuilder builder) =>
        GetChunkPrevious(builder) != null;

    static bool TryGetSingleChunk(this StringBuilder builder, out ReadOnlyMemory<char> single)
    {
        if (HasMultipleChunks(builder))
        {
            single = null;
            return false;
        }

        var chunks = builder.GetChunks();
        var enumerator = chunks.GetEnumerator();
        if (enumerator.MoveNext())
        {
            single = enumerator.Current;
            return true;
        }

        single = null;
        return false;
    }
#else

    static bool TryGetSingleChunk(this StringBuilder builder, out ReadOnlyMemory<char> single)
    {
        var chunks = builder.GetChunks();
        var enumerator = chunks.GetEnumerator();
        if (enumerator.MoveNext())
        {
            single = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                return true;
            }
        }

        single = null;
        return false;
    }
#endif

    static void ReplaceAllSingleChunk<TContext>(
        StringBuilder builder,
        CharSpan span,
        int maxLength,
        TContext context,
        MatchHandler<TContext> matcher)
    {
        List<Match> matches = [];

        for (var i = 0; i < span.Length; i++)
        {
            // Quick character check to skip positions that can't match
            var ch = span[i];
            if (ch is '\n' or '\r')
            {
                continue;
            }

            // Get window at current position
            var remainingLength = span.Length - i;
            var windowLength = Math.Min(maxLength, remainingLength);
            var window = span.Slice(i, windowLength);

            var potentialMatch = matcher(window, i, context);

            if (potentialMatch == null)
            {
                continue;
            }

            var match = potentialMatch.Value;
            matches.Add(new(i, match.Length, match.Replacement));

            // Skip past the match
            i += match.Length - 1;
        }

        // Apply matches in descending position order to maintain correct indices
        foreach (var match in matches.OrderByDescending(_ => _.Index))
        {
            builder.Overwrite(match.Value, match.Index, match.Length);
        }
    }

    static void ReplaceAllMultiChunk<TContext>(
        StringBuilder builder,
        int maxLength,
        TContext context,
        MatchHandler<TContext> matcher)
    {
        Span<char> buffer = stackalloc char[maxLength];
        List<Match> matches = [];
        var position = 0;

        foreach (var chunk in builder.GetChunks())
        {
            for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
            {
                // Quick character check to skip positions that can't match
                var ch = chunk.Span[chunkIndex];
                if (ch is '\n' or '\r')
                {
                    continue;
                }

                var absolutePosition = position + chunkIndex;

                // Build content window starting at current position
                var windowSlice = FillBuffer(builder, absolutePosition, buffer);

                var potentialMatch = matcher(windowSlice, absolutePosition, context);

                if (potentialMatch == null)
                {
                    continue;
                }

                var match = potentialMatch.Value;
                matches.Add(new(absolutePosition, match.Length, match.Replacement));

                // Skip past the match
                var skipAmount = match.Length - 1;
                if (skipAmount <= 0)
                {
                    continue;
                }

                var remaining = chunk.Length - chunkIndex - 1;
                var toSkip = Math.Min(skipAmount, remaining);
                chunkIndex += toSkip;
            }

            position += chunk.Length;
        }

        // Apply matches in descending position order to maintain correct indices
        foreach (var match in matches.OrderByDescending(_ => _.Index))
        {
            builder.Overwrite(match.Value, match.Index, match.Length);
        }
    }

    static Span<char> FillBuffer(StringBuilder builder, int start, Span<char> buffer)
    {
        var bufferIndex = 0;
        var currentPosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkEnd = currentPosition + chunk.Length;

            // Skip chunks before our start position
            if (chunkEnd <= start)
            {
                currentPosition = chunkEnd;
                continue;
            }

            // Determine where to start in this chunk
            var chunkStart = start > currentPosition ? start - currentPosition : 0;

            // Copy what we can from this chunk
            var destinationSlice = buffer[bufferIndex..];
            var toCopy = Math.Min(chunk.Length - chunkStart, destinationSlice.Length);

            chunk.Span.Slice(chunkStart, toCopy).CopyTo(destinationSlice);
            bufferIndex += toCopy;

            // If buffer is full, we're done
            if (bufferIndex >= buffer.Length)
            {
                break;
            }

            currentPosition = chunkEnd;
        }

        return buffer[..bufferIndex];
    }

    /// <summary>
    /// Callback for checking if content matches and should be replaced.
    /// </summary>
    /// <param name="content">The current window content to check</param>
    /// <param name="absolutePosition">Absolute position in the StringBuilder where this content starts</param>
    /// <param name="context">User-provided context</param>
    /// <returns>Match result indicating if a match was found and replacement details</returns>
    public delegate MatchResult? MatchHandler<in TContext>(
        CharSpan content,
        int absolutePosition,
        TContext context);
}

/// <summary>
/// Result of a match check operation.
/// </summary>
readonly struct MatchResult(int length, string replacement)
{
    public readonly int Length = length;
    public readonly string Replacement = replacement;
}

readonly struct Match(int index, int length, string value)
{
    public readonly int Index = index;
    public readonly int Length = length;
    public readonly string Value = value;
}