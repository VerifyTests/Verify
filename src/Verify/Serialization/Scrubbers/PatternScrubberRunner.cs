namespace VerifyTests;

static class PatternScrubberRunner
{
    static readonly Dictionary<string, object> emptyContext = new();

    public static void Run(StringBuilder builder, PatternScrubber scrubber, Counter counter)
    {
        if (builder.Length == 0)
        {
            return;
        }

        var source = builder.ToString();
        var chunks = new List<ScrubberChunk>();
        PatternWalker.Walk(source.AsSpan(), [scrubber], counter, emptyContext, chunks);
        builder.Clear();
        PatternWalker.Stitch(source.AsSpan(), chunks, builder);
    }
}
