
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class CrossChunkMatcherBenchmarks
{
    StringBuilder smallBuilder = null!;
    StringBuilder mediumBuilder = null!;
    StringBuilder largeBuilder = null!;
    StringBuilder manyMatchesBuilder = null!;
    StringBuilder crossChunkBuilder = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Small: ~1KB with a few chunks
        smallBuilder = new();
        for (var i = 0; i < 10; i++)
        {
            smallBuilder.AppendLine("Hello <TAG>world</TAG> this is");
            smallBuilder.AppendLine("a test with some <TAG>patterns</TAG> to match.");
        }

        // Medium: ~10KB with more chunks
        mediumBuilder = new();
        for (var i = 0; i < 100; i++)
        {
            mediumBuilder.AppendLine("Hello <TAG>world</TAG> this is");
            mediumBuilder.AppendLine("a test with some <TAG>patterns</TAG> to match.");
        }

        // Large: ~100KB with many chunks
        largeBuilder = new();
        for (var i = 0; i < 1000; i++)
        {
            largeBuilder.AppendLine("Hello <TAG>world</TAG> this is");
            largeBuilder.AppendLine("a test with some <TAG>patterns</TAG> to match.");
        }

        // Many matches: Lots of patterns to replace
        manyMatchesBuilder = new();
        for (var i = 0; i < 500; i++)
        {
            manyMatchesBuilder.AppendLine("<TAG>");
        }

        // Force cross-chunk pattern matching by creating multiple chunks
        // Pattern spans across chunk boundaries
        crossChunkBuilder = new();
        for (var i = 0; i < 100; i++)
        {
            // Create chunks where <TAG> might span boundaries
            crossChunkBuilder.Append("Hello <T");
            crossChunkBuilder.Append("AG>world</T");
            crossChunkBuilder.AppendLine("AG> test");
        }
    }

    // Baseline benchmarks (without skipChar)
    [Benchmark(Baseline = true)]
    public void Small_FewMatches_Baseline()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void Small_FewMatches_WithSkipChar()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void Medium_FewMatches_Baseline()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void Medium_FewMatches_WithSkipChar()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void Large_FewMatches_Baseline()
    {
        var builder = new StringBuilder(largeBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void Large_FewMatches_WithSkipChar()
    {
        var builder = new StringBuilder(largeBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void NoMatches_Baseline()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void NoMatches_WithSkipChar()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
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
    public void ComplexPattern_Baseline()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 100,
            context: (string?) null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>world</TAG>"))
                {
                    return new MatchResult(16, "[COMPLEX_MATCH]");
                }

                return null;
            });
    }

    [Benchmark]
    public void ComplexPattern_WithSkipChar()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 100,
            context: (string?) null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>world</TAG>"))
                {
                    return new MatchResult(16, "[COMPLEX_MATCH]");
                }

                return null;
            });
    }
}
