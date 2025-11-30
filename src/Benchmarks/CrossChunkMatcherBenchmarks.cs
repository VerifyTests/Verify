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
            smallBuilder.Append("Hello <TAG>world</TAG> this is ");
            smallBuilder.Append("a test with some <TAG>patterns</TAG> to match. ");
        }

        // Medium: ~10KB with more chunks
        mediumBuilder = new();
        for (var i = 0; i < 100; i++)
        {
            mediumBuilder.Append("Hello <TAG>world</TAG> this is ");
            mediumBuilder.Append("a test with some <TAG>patterns</TAG> to match. ");
        }

        // Large: ~100KB with many chunks
        largeBuilder = new();
        for (var i = 0; i < 1000; i++)
        {
            largeBuilder.Append("Hello <TAG>world</TAG> this is ");
            largeBuilder.Append("a test with some <TAG>patterns</TAG> to match. ");
        }

        // Many matches: Lots of patterns to replace
        manyMatchesBuilder = new();
        for (var i = 0; i < 500; i++)
        {
            manyMatchesBuilder.Append("<TAG>");
        }

        // Force cross-chunk pattern matching by creating multiple chunks
        // Pattern spans across chunk boundaries
        crossChunkBuilder = new();
        for (var i = 0; i < 100; i++)
        {
            // Create chunks where <TAG> might span boundaries
            crossChunkBuilder.Append("Hello <T");
            crossChunkBuilder.Append("AG>world</T");
            crossChunkBuilder.Append("AG> test ");
        }
    }

    [Benchmark]
    public void Small_FewMatches()
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
    public void Medium_FewMatches()
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
    public void Large_FewMatches()
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
    public void ManyMatches()
    {
        var builder = new StringBuilder(manyMatchesBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 10,
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
    public void ComplexPattern_LargeWindow()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 100,
            context: (string?) null,
            matcher: static (content, _, _) =>
            {
                // More complex pattern matching with larger window
                if (content.StartsWith("<TAG>world</TAG>"))
                {
                    return new MatchResult(16, "[COMPLEX_MATCH]");
                }

                return null;
            });
    }

    [Benchmark]
    public void MultiplePatterns()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 30,
            context: (string?) null,
            matcher: static (content, _, _) =>
            {
                if (content.StartsWith("<TAG>world</TAG>"))
                {
                    return new MatchResult(16, "[WORLD]");
                }

                if (content.StartsWith("<TAG>patterns</TAG>"))
                {
                    return new MatchResult(19, "[PATTERN]");
                }

                return null;
            });
    }

    [Benchmark]
    public void NoMatches()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 20,
            context: (string?) null,
            matcher: static (content, _, _) =>
            {
                // Pattern that will never match
                if (content.StartsWith("<NOMATCH>"))
                {
                    return new MatchResult(9, "[REPLACED]");
                }

                return null;
            });
    }

    [Benchmark]
    public void CrossChunkPatterns()
    {
        var builder = new StringBuilder(crossChunkBuilder.ToString());
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
}