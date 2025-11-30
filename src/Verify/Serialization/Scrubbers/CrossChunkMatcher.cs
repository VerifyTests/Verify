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

        Span<char> buffer = stackalloc char[maxLength];
        List<Match> matches = [];
        var position = 0;

        foreach (var chunk in builder.GetChunks())
        {
            for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
            {
                var absolutePosition = position + chunkIndex;

                // Build content window starting at current position
                var bufferLength = FillBuffer(builder, absolutePosition, buffer);

                // Check for match at this position
                var windowSlice = buffer[..bufferLength];
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
    static int FillBuffer(StringBuilder builder, int start, Span<char> buffer)
    {
        var bufferIndex = 0;
        var currentPosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;
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

            chunkSpan.Slice(chunkStart, toCopy).CopyTo(destinationSlice);
            bufferIndex += toCopy;

            // If buffer is full, we're done
            if (bufferIndex >= buffer.Length)
            {
                break;
            }

            currentPosition = chunkEnd;
        }

        return bufferIndex;
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