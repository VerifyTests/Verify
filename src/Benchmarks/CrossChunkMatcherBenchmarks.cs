[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class CrossChunkMatcherBenchmarks
{
    StringBuilder singleChunkSmall = null!;
    StringBuilder singleChunkMedium = null!;
    StringBuilder singleChunkLarge = null!;
    StringBuilder multiChunkSmall = null!;
    StringBuilder multiChunkMedium = null!;
    StringBuilder multiChunkLarge = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Single-chunk builders (created from string - fast path)
        var smallText = BuildText(10);
        var mediumText = BuildText(100);
        var largeText = BuildText(1000);

        singleChunkSmall = new(smallText);
        singleChunkMedium = new(mediumText);
        singleChunkLarge = new(largeText);

        // Multi-chunk builders (built incrementally to force multiple chunks)
        multiChunkSmall = BuildMultiChunk(10);
        multiChunkMedium = BuildMultiChunk(100);
        multiChunkLarge = BuildMultiChunk(1000);
    }

    static string BuildText(int iterations)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < iterations; i++)
        {
            builder.AppendLine("Hello <TAG>world</TAG> this is");
            builder.AppendLine("a test with some <TAG>patterns</TAG> to match.");
        }
        return builder.ToString();
    }

    static StringBuilder BuildMultiChunk(int iterations)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < iterations; i++)
        {
            // Each Append can create separate chunks
            builder.Append("Hello ");
            builder.Append("<TAG>");
            builder.Append("world");
            builder.Append("</TAG>");
            builder.AppendLine(" this is");
            builder.Append("a test with some ");
            builder.Append("<TAG>");
            builder.Append("patterns");
            builder.Append("</TAG>");
            builder.AppendLine(" to match.");
        }
        return builder;
    }

    // Single-chunk benchmarks (fast path)
    [Benchmark]
    public void SingleChunk_Small()
    {
        var builder = new StringBuilder(singleChunkSmall.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>"))
                {
                    return new MatchResult(5, "[REPLACED]");
                }
                return null;
            });
    }

    [Benchmark]
    public void SingleChunk_Medium()
    {
        var builder = new StringBuilder(singleChunkMedium.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>"))
                {
                    return new MatchResult(5, "[REPLACED]");
                }
                return null;
            });
    }

    [Benchmark]
    public void SingleChunk_Large()
    {
        var builder = new StringBuilder(singleChunkLarge.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>"))
                {
                    return new MatchResult(5, "[REPLACED]");
                }
                return null;
            });
    }

    // Multi-chunk benchmarks (complex path)
    [Benchmark]
    public void MultiChunk_Small()
    {
        var builder = new StringBuilder();
        foreach (var chunk in multiChunkSmall.GetChunks())
        {
            builder.Append(chunk.Span);
        }
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>"))
                {
                    return new MatchResult(5, "[REPLACED]");
                }
                return null;
            });
    }

    [Benchmark]
    public void MultiChunk_Medium()
    {
        var builder = new StringBuilder();
        foreach (var chunk in multiChunkMedium.GetChunks())
        {
            builder.Append(chunk.Span);
        }
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>"))
                {
                    return new MatchResult(5, "[REPLACED]");
                }
                return null;
            });
    }

    [Benchmark]
    public void MultiChunk_Large()
    {
        var builder = new StringBuilder();
        foreach (var chunk in multiChunkLarge.GetChunks())
        {
            builder.Append(chunk.Span);
        }
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>"))
                {
                    return new MatchResult(5, "[REPLACED]");
                }
                return null;
            });
    }

    // Edge cases
    [Benchmark]
    public void SingleChunk_NoMatches()
    {
        var builder = new StringBuilder(singleChunkMedium.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<NOMATCH>"))
                {
                    return new MatchResult(9, "[REPLACED]");
                }
                return null;
            });
    }

    [Benchmark]
    public void MultiChunk_NoMatches()
    {
        var builder = new StringBuilder();
        foreach (var chunk in multiChunkMedium.GetChunks())
        {
            builder.Append(chunk.Span);
        }
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<NOMATCH>"))
                {
                    return new MatchResult(9, "[REPLACED]");
                }
                return null;
            });
    }

    [Benchmark]
    public void SingleChunk_ComplexPattern()
    {
        var builder = new StringBuilder(singleChunkMedium.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 100,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>world</TAG>"))
                {
                    return new MatchResult(16, "[COMPLEX]");
                }
                return null;
            });
    }

    [Benchmark]
    public void MultiChunk_ComplexPattern()
    {
        var builder = new StringBuilder();
        foreach (var chunk in multiChunkMedium.GetChunks())
        {
            builder.Append(chunk.Span);
        }
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 100,
            context: (string?)null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>world</TAG>"))
                {
                    return new MatchResult(16, "[COMPLEX]");
                }
                return null;
            });
    }
}