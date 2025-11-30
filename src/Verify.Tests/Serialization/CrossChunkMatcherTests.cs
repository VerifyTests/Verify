public class CrossChunkMatcherTests
{
    [Fact]
    public Task SimpleWithinChunkMatch()
    {
        var builder = new StringBuilder("Hello World");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 5,
            context: "World",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "Universe");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task MultipleWithinChunkMatches()
    {
        var builder = new StringBuilder("foo bar foo baz foo");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 3,
            context: "foo",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "bar");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task CrossChunkMatch()
    {
        // Create a StringBuilder with multiple chunks
        var builder = new StringBuilder();
        builder.Append(new string('a', 8000)); // First chunk
        builder.Append("MATCH");
        builder.Append(new string('b', 8000)); // Force into new chunk

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 5,
            context: "MATCH",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "FOUND");
                }

                return null;
            });

        var result = builder.ToString();
        var matchPosition = result.IndexOf("FOUND", StringComparison.Ordinal);
        var surroundingContext = result.Substring(
            Math.Max(0, matchPosition - 10),
            Math.Min(25, result.Length - matchPosition + 10));

        return Verify(new
        {
            MatchFound = matchPosition >= 0,
            Position = matchPosition,
            Context = surroundingContext
        });
    }

    [Fact]
    public Task NoMatches()
    {
        var builder = new StringBuilder("Hello World");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 10,
            context: "NotFound",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "Replaced");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task EmptyStringBuilder()
    {
        var builder = new StringBuilder();

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 5,
            context: "test",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "replaced");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task MatchAtStart()
    {
        var builder = new StringBuilder("TargetString");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 6,
            context: "Target",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "Result");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task MatchAtEnd()
    {
        var builder = new StringBuilder("StringTarget");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 6,
            context: "Target",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "Result");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task OverlappingMatches()
    {
        var builder = new StringBuilder("aaaa");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 2,
            context: "aa",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "bb");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task DifferentReplacementLengths()
    {
        var builder = new StringBuilder("short x long y");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 5,
            context: (Short: "short", Long: "long"),
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Short.Length &&
                    content[..context.Short.Length].SequenceEqual(context.Short))
                {
                    return new(context.Short.Length, "replaced");
                }

                if (content.Length >= context.Long.Length &&
                    content[..context.Long.Length].SequenceEqual(context.Long))
                {
                    return new(context.Long.Length, "r");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task SingleCharacterMatch()
    {
        var builder = new StringBuilder("a b a c a");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 1,
            context: 'a',
            matcher: static (content, _, context) =>
            {
                if (content.Length >= 1 && content[0] == context)
                {
                    return new(1, "x");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task LargeStringBuilderWithMultipleChunks()
    {
        var builder = new StringBuilder();
        // Create multiple chunks with patterns
        for (var i = 0; i < 5; i++)
        {
            builder.Append(new string('x', 7000));
            builder.Append("PATTERN");
        }

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 7,
            context: "PATTERN",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "MATCH");
                }

                return null;
            });

        var result = builder.ToString();
        var matchCount = 0;
        var index = 0;
        while ((index = result.IndexOf("MATCH", index, StringComparison.Ordinal)) != -1)
        {
            matchCount++;
            index += "MATCH".Length;
        }

        return Verify(new {MatchCount = matchCount});
    }

    [Fact]
    public Task PatternSplitAcrossChunkBoundary()
    {
        var builder = new StringBuilder();
        // Create a pattern that will be split across chunk boundary
        // First chunk ends with "PAT", second chunk starts with "TERN"
        var firstChunk = new string('a', 8000) + "PAT";
        var secondChunk = "TERN" + new string('b', 8000);

        builder.Append(firstChunk);
        builder.Append(secondChunk);

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 7,
            context: "PATTERN",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "SUCCESS");
                }

                return null;
            });

        var result = builder.ToString();
        var matchPosition = result.IndexOf("SUCCESS", StringComparison.Ordinal);

        return Verify(new
        {
            MatchFound = matchPosition >= 0,
            ExpectedPosition = firstChunk.Length - 3,
            ActualPosition = matchPosition
        });
    }

    [Fact]
    public Task ConsecutiveMatches()
    {
        var builder = new StringBuilder("ABABAB");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 2,
            context: "AB",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "XY");
                }

                return null;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task PatternSpanningThreeChunks()
    {
        // Pattern "ABCDEFGH" spans 3 chunks
        var builder = new StringBuilder();

        // Chunk 1: ends with "ABC"
        builder.Append(new string('x', 8000) + "ABC");
        // Chunk 2: contains "DEF"
        builder.Append("DEF");
        // Chunk 3: starts with "GH"
        builder.Append("GH" + new string('y', 8000));

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 8,
            context: "ABCDEFGH",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "FOUND!!!");
                }

                return null;
            });

        var result = builder.ToString();
        var matchFound = result.Contains("FOUND!!!");
        var originalPatternExists = result.Contains("ABCDEFGH");

        return Verify(new
        {
            MatchFound = matchFound,
            OriginalPatternExists = originalPatternExists,
            Note = "Pattern spanning 3 chunks should now be detected"
        });
    }

    [Fact]
    public Task PatternSpanningThreeChunks_AlternativeLayout()
    {
        // Another layout: AB | CDEF | GH
        var builder = new StringBuilder();

        builder.Append(new string('x', 8000) + "AB");
        builder.Append("CDEF");
        builder.Append("GH" + new string('y', 8000));

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 8,
            context: "ABCDEFGH",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "FOUND!!!");
                }

                return null;
            });

        var result = builder.ToString();

        return Verify(new
        {
            MatchFound = result.Contains("FOUND!!!"),
            OriginalPatternExists = result.Contains("ABCDEFGH"),
            Note = "Pattern spanning 3 chunks should now be detected"
        });
    }

    [Fact]
    public Task PatternSpanningFourChunks()
    {
        // Extreme case: A | BC | DE | FGH
        var builder = new StringBuilder();

        builder.Append(new string('x', 8000) + "A");
        builder.Append("BC");
        builder.Append("DE");
        builder.Append("FGH" + new string('y', 8000));

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 8,
            context: "ABCDEFGH",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "SUCCESS!");
                }

                return null;
            });

        var result = builder.ToString();

        return Verify(new
        {
            MatchFound = result.Contains("SUCCESS!"),
            OriginalPatternExists = result.Contains("ABCDEFGH"),
            Note = "Pattern spanning 4 chunks should now be detected"
        });
    }

    [Fact]
    public Task MultipleMatchesAcrossChunks()
    {
        var builder = new StringBuilder();

        // Multiple patterns across chunks
        builder.Append(new string('x', 7000) + "PAT");
        builder.Append("TERN1" + new string('y', 7000) + "PAT");
        builder.Append("TERN2" + new string('z', 7000));

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 7,
            context: "PATTERN",
            matcher: static (content, _, context) =>
            {
                if (content.Length >= context.Length &&
                    content[..context.Length].SequenceEqual(context))
                {
                    return new(context.Length, "MATCH");
                }

                return null;
            });

        var result = builder.ToString();
        var matchCount = 0;
        var index = 0;
        while ((index = result.IndexOf("MATCH", index, StringComparison.Ordinal)) != -1)
        {
            matchCount++;
            index += "MATCH".Length;
        }

        return Verify(new {MatchCount = matchCount});
    }

    [Fact]
    public Task VariableLengthMatches()
    {
        var builder = new StringBuilder("cat dog bird elephant");

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 8,
            context: new[] {"cat", "dog", "bird", "elephant"},
            matcher: static (content, _, context) =>
            {
                foreach (var word in context)
                {
                    if (content.Length >= word.Length &&
                        content[..word.Length].SequenceEqual(word))
                    {
                        return new(word.Length, "animal");
                    }
                }

                return null;
            });

        return Verify(builder.ToString());
    }
}