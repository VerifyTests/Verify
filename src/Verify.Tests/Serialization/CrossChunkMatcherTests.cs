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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "Universe"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "bar"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, carryoverBuffer, buffer, carryoverIndex, remainingInCarryover, currentChunkSpan, absoluteStartPosition, context, addMatch) =>
            {
                var neededFromCurrent = context.Length - remainingInCarryover;
                if (neededFromCurrent <= 0 || neededFromCurrent > currentChunkSpan.Length)
                {
                    return;
                }

                // Combine carryover + current chunk into buffer
                carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(buffer);
                currentChunkSpan[..neededFromCurrent].CopyTo(buffer[remainingInCarryover..]);

                if (buffer[..context.Length].SequenceEqual(context))
                {
                    addMatch(new(absoluteStartPosition, context.Length, "FOUND"));
                }
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "FOUND"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "Replaced"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (_, _, _, _, _, _) => 1);

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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "Result"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "Result"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "bb"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Short.Length &&
                    chunkSpan.Slice(chunkIndex, context.Short.Length).SequenceEqual(context.Short))
                {
                    addMatch(new(absoluteIndex, context.Short.Length, "replaced"));
                    return context.Short.Length;
                }

                if (remaining >= context.Long.Length &&
                    chunkSpan.Slice(chunkIndex, context.Long.Length).SequenceEqual(context.Long))
                {
                    addMatch(new(absoluteIndex, context.Long.Length, "r"));
                    return context.Long.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (_, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                if (chunkSpan[chunkIndex] == context)
                {
                    addMatch(new(absoluteIndex, 1, "x"));
                }

                return 1;
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
            onCrossChunk: static (_, carryoverBuffer, buffer, carryoverIndex, remainingInCarryover, currentChunkSpan, absoluteStartPosition, context, addMatch) =>
            {
                var neededFromCurrent = context.Length - remainingInCarryover;
                if (neededFromCurrent <= 0 || neededFromCurrent > currentChunkSpan.Length)
                {
                    return;
                }

                carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(buffer);
                currentChunkSpan[..neededFromCurrent].CopyTo(buffer.Slice(remainingInCarryover));

                if (buffer[..context.Length].SequenceEqual(context))
                {
                    addMatch(new(absoluteStartPosition, context.Length, "MATCH"));
                }
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "MATCH"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, carryoverBuffer, buffer, carryoverIndex, remainingInCarryover, currentChunkSpan, absoluteStartPosition, context, addMatch) =>
            {
                var neededFromCurrent = context.Length - remainingInCarryover;
                if (neededFromCurrent <= 0 || neededFromCurrent > currentChunkSpan.Length)
                {
                    return;
                }

                carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(buffer);
                currentChunkSpan[..neededFromCurrent].CopyTo(buffer.Slice(remainingInCarryover));

                if (buffer[..context.Length].SequenceEqual(context))
                {
                    addMatch(new(absoluteStartPosition, context.Length, "SUCCESS"));
                }
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "SUCCESS"));
                    return context.Length;
                }

                return 1;
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
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (chunk, chunkSpan, chunkIndex, absoluteIndex, context, addMatch) =>
            {
                var remaining = chunk.Length - chunkIndex;
                if (remaining >= context.Length &&
                    chunkSpan.Slice(chunkIndex, context.Length).SequenceEqual(context))
                {
                    addMatch(new(absoluteIndex, context.Length, "XY"));
                    return context.Length;
                }

                return 1;
            });

        return Verify(builder.ToString());
    }

    [Fact]
    public Task SkipAheadFunctionality()
    {
        var builder = new StringBuilder("one two three four");
        var positions = new List<int>();

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 5,
            context: positions,
            onCrossChunk: static (_, _, _, _, _, _, _, _, _) =>
            {
            },
            onWithinChunk: static (_, _, _, absoluteIndex, context, _) =>
            {
                context.Add(absoluteIndex);
                // Skip ahead by 3 positions every time
                return 3;
            });

        return Verify(positions);
    }
}