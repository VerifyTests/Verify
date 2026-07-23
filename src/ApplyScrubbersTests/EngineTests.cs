public class EngineTests
{
    [Fact]
    public void Quarantine_ReplacementNotSeenByOtherScrubbers()
    {
        // Equal max lengths, so registration order applies: first replaces abc
        // with xyz, and the quarantined xyz must not be re-matched by the second
        var result = EngineRunner.Run(
            "abc",
            Scrubber.Replace("abc", "xyz"),
            Scrubber.Replace("xyz", "!!!"));
        Assert.Equal("xyz", result);
    }

    [Fact]
    public void Quarantine_ReplacementNotRescannedBySameScrubber()
    {
        // A replacement containing its own find must not recurse
        var result = EngineRunner.Run("aa", Scrubber.Replace("aa", "aaaa"));
        Assert.Equal("aaaa", result);
    }

    [Fact]
    public void ZeroCopy_NoMatches()
    {
        var input = "no matches here";
        var result = EngineRunner.Run(input, Scrubber.Replace("missing", "x"));
        Assert.Same(input, result);
    }

    [Fact]
    public void Newlines_CrLfCollapsed()
    {
        var result = EngineRunner.Run("a\r\nb\rc", Scrubber.Replace("missing", "x"));
        Assert.Equal("a\nb\nc", result);
    }

    [Fact]
    public void Newlines_ReplacementNormalized()
    {
        var result = EngineRunner.Run(
            "token",
            Scrubber.Match((CharSpan segment, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
            {
                index = segment.IndexOf("token".AsSpan());
                if (index < 0)
                {
                    length = 0;
                    replacement = null;
                    return false;
                }

                length = 5;
                replacement = "x\r\ny";
                return true;
            }));
        Assert.Equal("x\ny", result);
    }

    [Fact]
    public void EmptyInput()
    {
        var input = string.Empty;
        var result = EngineRunner.Run(input, Scrubber.Replace("a", "b"));
        Assert.Same(input, result);
    }

    [Fact]
    public void Split_PrefixAndSuffixPreserved()
    {
        var result = EngineRunner.Run("xxabcxx", Scrubber.Replace("abc", "Y"));
        Assert.Equal("xxYxx", result);
    }

    [Fact]
    public void EmptyReplacement_RemovesText()
    {
        var result = EngineRunner.Run("a-remove-b", Scrubber.Replace("-remove-", ""));
        Assert.Equal("ab", result);
    }

    [Fact]
    public void MultipleMatches_SameScrubber()
    {
        var result = EngineRunner.Run("abc abc abc", Scrubber.Replace("abc", "Y"));
        Assert.Equal("Y Y Y", result);
    }

    [Fact]
    public void WordBoundary_RejectsAdjacentLetterOrDigit()
    {
        var result = EngineRunner.Run(
            "name names 1name name",
            Scrubber.Replace("name", "X", requireWordBoundary: true));
        Assert.Equal("X names 1name X", result);
    }

    [Fact]
    public void WordBoundary_ReplacementChunkIsNeighbor()
    {
        // First scrubber replaces ab with 12. The second requires a word boundary;
        // the preceding chunk now ends with '2' (a digit), so cd must not match.
        var result = EngineRunner.Run(
            "abcd",
            Scrubber.Replace("ab", "12"),
            Scrubber.Replace("cd", "YZ", requireWordBoundary: true));
        Assert.Equal("12cd", result);
    }

    [Fact]
    public void MinLength_SkipsShortValues()
    {
        var input = "ab";
        var result = EngineRunner.Run(input, Scrubber.Replace("abc", "x"));
        Assert.Same(input, result);
    }

    [Fact]
    public void MultiPair_LongestWinsAtSamePosition()
    {
        var result = EngineRunner.Run(
            "abc",
            Scrubber.Replace(StringComparison.Ordinal, false, ("ab", "SHORT"), ("abc", "LONG")));
        Assert.Equal("LONG", result);
    }

    [Fact]
    public void MultiPair_EarlierPositionWins()
    {
        var result = EngineRunner.Run(
            "abc",
            Scrubber.Replace(StringComparison.Ordinal, false, ("bc", "B"), ("c", "C")));
        Assert.Equal("aB", result);
    }

    [Fact]
    public void Comparison_OrdinalIgnoreCase()
    {
        var result = EngineRunner.Run(
            "ABC abc",
            Scrubber.Replace("abc", "x", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("x x", result);
    }

    [Fact]
    public void Validation_FindWithNewlineThrows() =>
        Assert.Throws<ArgumentException>(() => Scrubber.Replace("a\nb", "x"));

    [Fact]
    public void Validation_WindowBoundsThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Scrubber.Window(0, 5, (CharSpan _, Counter _, IReadOnlyDictionary<string, object> _) => null));
        Assert.Throws<ArgumentOutOfRangeException>(() => Scrubber.Window(5, 4, (CharSpan _, Counter _, IReadOnlyDictionary<string, object> _) => null));
    }
}
