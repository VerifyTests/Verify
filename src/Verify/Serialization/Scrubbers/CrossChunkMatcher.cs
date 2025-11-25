/// <summary>
/// Helper for matching and replacing patterns in StringBuilder that may span across chunk boundaries.
/// </summary>
static class CrossChunkMatcher
{
    /// <summary>
    /// Finds all matches in a StringBuilder (handling patterns spanning chunk boundaries) and applies replacements.
    /// </summary>
    /// <param name="builder">The StringBuilder to search and modify</param>
    /// <param name="carryoverSize">Size of carryover buffer (typically maxPatternLength - 1)</param>
    /// <param name="context">User context passed to callbacks</param>
    /// <param name="onCrossChunk">Called for each potential cross-chunk match position</param>
    /// <param name="onWithinChunk">Called for each position within a chunk</param>
    public static void ReplaceAll<TContext>(
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
        List<Match> matches  = [];
        var addMatch = matches.Add;
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
                        context,
                        addMatch);
                }
            }

            // Process matches entirely within this chunk
            var chunkIndex = 0;
            while (chunkIndex < chunk.Length)
            {
                var absoluteIndex = absolutePosition + chunkIndex;
                var skipAhead = onWithinChunk(chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch);
                chunkIndex += skipAhead > 0 ? skipAhead : 1;
            }

            // Save last N chars for next iteration
            carryoverLength = Math.Min(carryoverSize, chunk.Length);
            chunkSpan.Slice(chunk.Length - carryoverLength, carryoverLength).CopyTo(carryoverBuffer);

            previousChunkAbsoluteEnd = absolutePosition + chunk.Length;
            absolutePosition += chunk.Length;
        }

        // Apply matches in descending position order
        foreach (var match in matches.OrderByDescending(_ => _.Index))
        {
            builder.Overwrite(match.Value, match.Index, match.Length);
        }
    }

    /// <summary>
    /// Callback for processing potential cross-chunk matches.
    /// </summary>
    public delegate void CrossChunkHandler<TContext>(
        StringBuilder builder,
        Span<char> carryoverBuffer,
        int carryoverIndex,
        int remainingInCarryover,
        CharSpan currentChunkSpan,
        int absoluteStartPosition,
        TContext context,
        Action<Match> addMatch);

    /// <summary>
    /// Callback for processing positions within a chunk.
    /// </summary>
    /// <returns>Number of positions to skip ahead (0 or 1 for normal iteration, more to skip past a match)</returns>
    public delegate int WithinChunkHandler<TContext>(
        ReadOnlyMemory<char> chunk,
        CharSpan chunkSpan,
        int chunkIndex,
        int absoluteIndex,
        TContext context,
        Action<Match> addMatch);
}


readonly struct Match(int index, int length, string value)
{
    public readonly int Index = index;
    public readonly int Length = length;
    public readonly string Value = value;
}