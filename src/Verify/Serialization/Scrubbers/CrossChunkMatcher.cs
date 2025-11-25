/// <summary>
/// Helper for matching patterns in StringBuilder that may span across chunk boundaries.
/// </summary>
static class CrossChunkMatcher
{
    /// <summary>
    /// Iterates through StringBuilder chunks, invoking callbacks for potential matches
    /// both within chunks and spanning chunk boundaries.
    /// </summary>
    /// <param name="builder">The StringBuilder to search</param>
    /// <param name="carryoverSize">Size of carryover buffer (typically maxPatternLength - 1)</param>
    /// <param name="context">User context passed to callbacks</param>
    /// <param name="onCrossChunk">Called for each potential cross-chunk match position</param>
    /// <param name="onWithinChunk">Called for each position within a chunk</param>
    public static void ProcessChunks<TContext>(
        StringBuilder builder,
        int carryoverSize,
        TContext context,
        CrossChunkHandler<TContext> onCrossChunk,
        WithinChunkHandler<TContext> onWithinChunk)
    {
        Span<char> carryoverBuffer = stackalloc char[carryoverSize];
        var carryoverLength = 0;
        var previousChunkAbsoluteEnd = 0;
        var absolutePosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;

            // Check for matches spanning from previous chunk to current chunk
            if (carryoverLength > 0)
            {
                for (var carryoverIndex = 0; carryoverIndex < carryoverLength; carryoverIndex++)
                {
                    var remainingInCarryover = carryoverLength - carryoverIndex;
                    var startPosition = previousChunkAbsoluteEnd - carryoverLength + carryoverIndex;

                    onCrossChunk(
                        builder,
                        carryoverBuffer,
                        carryoverIndex,
                        remainingInCarryover,
                        chunkSpan,
                        startPosition,
                        context);
                }
            }

            // Process matches entirely within this chunk
            var chunkIndex = 0;
            while (chunkIndex < chunk.Length)
            {
                var absoluteIndex = absolutePosition + chunkIndex;
                var skipAhead = onWithinChunk(chunk, chunkSpan, chunkIndex, absoluteIndex, context);
                chunkIndex += skipAhead > 0 ? skipAhead : 1;
            }

            // Save last N chars for next iteration
            carryoverLength = Math.Min(carryoverSize, chunk.Length);
            chunkSpan.Slice(chunk.Length - carryoverLength, carryoverLength).CopyTo(carryoverBuffer);

            previousChunkAbsoluteEnd = absolutePosition + chunk.Length;
            absolutePosition += chunk.Length;
        }
    }

    /// <summary>
    /// Applies matches to a StringBuilder in descending position order.
    /// </summary>
    public static void ApplyMatches<TMatch>(
        StringBuilder builder,
        List<TMatch> matches,
        Func<TMatch, int> getIndex,
        Func<TMatch, int> getLength,
        Func<TMatch, string> getValue)
    {
        foreach (var match in matches.OrderByDescending(getIndex))
        {
            builder.Overwrite(getValue(match), getIndex(match), getLength(match));
        }
    }

    /// <summary>
    /// Callback for processing potential cross-chunk matches.
    /// </summary>
    /// <param name="builder">The StringBuilder being processed</param>
    /// <param name="carryoverBuffer">Buffer containing end of previous chunk</param>
    /// <param name="carryoverIndex">Starting index within carryover buffer</param>
    /// <param name="remainingInCarryover">Characters remaining from carryoverIndex to end</param>
    /// <param name="currentChunkSpan">Span of the current chunk</param>
    /// <param name="absoluteStartPosition">Absolute position in StringBuilder where potential match starts</param>
    /// <param name="context">User context</param>
    public delegate void CrossChunkHandler<TContext>(
        StringBuilder builder,
        Span<char> carryoverBuffer,
        int carryoverIndex,
        int remainingInCarryover,
        CharSpan currentChunkSpan,
        int absoluteStartPosition,
        TContext context);

    /// <summary>
    /// Callback for processing positions within a chunk.
    /// </summary>
    /// <param name="chunk">The current chunk memory</param>
    /// <param name="chunkSpan">Span of the current chunk</param>
    /// <param name="chunkIndex">Current index within the chunk</param>
    /// <param name="absoluteIndex">Absolute position in StringBuilder</param>
    /// <param name="context">User context</param>
    /// <returns>Number of positions to skip ahead (0 or 1 for normal iteration, more to skip past a match)</returns>
    public delegate int WithinChunkHandler<TContext>(
        ReadOnlyMemory<char> chunk,
        CharSpan chunkSpan,
        int chunkIndex,
        int absoluteIndex,
        TContext context);
}
