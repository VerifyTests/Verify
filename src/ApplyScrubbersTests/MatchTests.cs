public class MatchTests
{
    static SegmentMatch DigitRunMatcher { get; } =
        (CharSpan segment, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
        {
            for (index = 0; index < segment.Length; index++)
            {
                if (!char.IsDigit(segment[index]))
                {
                    continue;
                }

                var end = index;
                while (end < segment.Length &&
                       char.IsDigit(segment[end]))
                {
                    end++;
                }

                length = end - index;
                replacement = "#";
                return true;
            }

            length = 0;
            replacement = null;
            return false;
        };

    [Fact]
    public void ReinvokedPastEachMatch()
    {
        var result = EngineRunner.Run("a1b22c333", Scrubber.Match(DigitRunMatcher));
        Assert.Equal("a#b#c#", result);
    }

    [Fact]
    public void MinLength_SkipsShortSegments()
    {
        var invoked = false;
        EngineRunner.Run(
            "abc",
            Scrubber.Match(
                (CharSpan _, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
                {
                    invoked = true;
                    index = 0;
                    length = 0;
                    replacement = null;
                    return false;
                },
                minLength: 5));
        Assert.False(invoked);
    }

    [Fact]
    public void InvalidRange_Throws()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            EngineRunner.Run(
                "abc",
                Scrubber.Match((CharSpan _, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
                {
                    index = 0;
                    length = 10;
                    replacement = "x";
                    return true;
                })));
        Assert.Contains("invalid range", exception.Message);
    }

    [Fact]
    public void NewlineSpanningMatch_Throws()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            EngineRunner.Run(
                "ab\ncd",
                Scrubber.Match((CharSpan _, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
                {
                    index = 0;
                    length = 5;
                    replacement = "x";
                    return true;
                })));
        Assert.Contains("line break", exception.Message);
    }

    [Fact]
    public void ContextAndCounter_FlowToMatcher()
    {
        using var counter = Counter.Start();
        Counter? seenCounter = null;
        IReadOnlyDictionary<string, object>? seenContext = null;
        EngineRunner.Run(
            "abc",
            counter,
            Scrubber.Match((CharSpan _, Counter matcherCounter, IReadOnlyDictionary<string, object> matcherContext, out int index, out int length, out string? replacement) =>
            {
                seenCounter = matcherCounter;
                seenContext = matcherContext;
                index = 0;
                length = 0;
                replacement = null;
                return false;
            }));
        Assert.Same(counter, seenCounter);
        Assert.Same(EngineRunner.EmptyContext, seenContext);
    }

    [Fact]
    public void UlidStyle_CounterNumbering()
    {
        // A fixed length token matcher using counter numbering, the Verify.Ulid shape
        using var counter = Counter.Start();
        var result = EngineRunner.Run(
            "01ARZ3NDEKTSV4RRFFQ69G5FAV then 01BX5ZZKBKACTAV9WEVGEMMVRZ",
            counter,
            Scrubber.Window(26, 26, (window, windowCounter, _) =>
            {
                foreach (var ch in window)
                {
                    if (!char.IsLetterOrDigit(ch))
                    {
                        return null;
                    }
                }

                return $"Ulid_{windowCounter.Next(new Guid(HashUlid(window), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0))}";
            }, requireWordBoundary: true));
        Assert.Equal("Ulid_1 then Ulid_2", result);
    }

    static int HashUlid(CharSpan window)
    {
        var hash = 17;
        foreach (var ch in window)
        {
            hash = unchecked((hash * 31) + ch);
        }

        return hash;
    }
}
