namespace VerifyTests;

static class PatternScrubberRunner
{
    static readonly Dictionary<string, object> emptyContext = new();

    public static void Run(StringBuilder builder, PatternScrubber scrubber, Counter counter)
    {
        var length = builder.Length;
        if (length == 0)
        {
            return;
        }

        var pool = ArrayPool<char>.Shared;
        var sourceBuffer = pool.Rent(length);
        try
        {
            builder.CopyTo(0, sourceBuffer, 0, length);
            var sourceSpan = sourceBuffer.AsSpan(0, length);

            var chunks = new List<ScrubberChunk>(8);
            PatternWalker.Walk(sourceSpan, [scrubber], counter, emptyContext, chunks);

            var hasReplacement = false;
            foreach (var chunk in chunks)
            {
                if (chunk.Replacement is not null)
                {
                    hasReplacement = true;
                    break;
                }
            }

            if (!hasReplacement)
            {
                return;
            }

            builder.Clear();
            PatternWalker.Stitch(sourceSpan, chunks, builder);
        }
        finally
        {
            pool.Return(sourceBuffer);
        }
    }
}
