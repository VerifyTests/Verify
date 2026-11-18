// A line transform is defined over a single line, so when one produces line
// breaks the remaining transforms must see each produced line on its own.
public class MultiLineTransformTests
{
    [Fact]
    public void LaterTransformSeesProducedLines()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(_ => _ == "a" ? "x\ny" : _),
            Scrubber.ReplaceLines(_ => _ == "x" ? "z" : _));
        Assert.Equal("z\ny", result);
    }

    [Fact]
    public void LaterTransformSeesEveryProducedLine()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(_ => _ == "a" ? "x\ny\nz" : _),
            Scrubber.ReplaceLines(_ => _.ToUpperInvariant()));
        Assert.Equal("X\nY\nZ", result);
    }

    [Fact]
    public void LaterTransformCanRemoveAProducedLine()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(line => line == "a" ? "keep\ndrop\nkeep2" : line),
            Scrubber.ReplaceLines(_ => _ == "drop" ? null : _));
        Assert.Equal("keep\nkeep2", result);
    }

    [Fact]
    public void RemovingEveryProducedLineRemovesTheLine()
    {
        var result = EngineRunner.Run(
            "a\nb",
            Scrubber.ReplaceLines(_ => _ == "a" ? "x\ny" : _),
            Scrubber.ReplaceLines(_ => _ is "x" or "y" ? null : _));
        Assert.Equal("b", result);
    }

    [Fact]
    public void ProducedLinesCanExpandAgain()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(_ => _ == "a" ? "x\ny" : _),
            Scrubber.ReplaceLines(_ => _ == "y" ? "1\n2" : _),
            Scrubber.ReplaceLines(_ => _ == "2" ? "end" : _));
        Assert.Equal("x\n1\nend", result);
    }

    [Fact]
    public void SpanTransformSeesProducedLines()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(_ => _.SequenceEqual("a".AsSpan()) ? LineResult.Replace("x\ny") : LineResult.Keep),
            Scrubber.ReplaceLines(_ => _.SequenceEqual("x".AsSpan()) ? LineResult.Replace("z") : LineResult.Keep));
        Assert.Equal("z\ny", result);
    }

    [Fact]
    public void TrailingTransformLeavesTextExactlyAsProduced()
    {
        // Nothing follows the expanding transform, so the text is untouched
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(_ => _ == "a" ? "x\ny\n" : _));
        Assert.Equal("x\ny\n", result);
    }

    [Fact]
    public void SurroundingLinesArePreserved()
    {
        var result = EngineRunner.Run(
            "before\na\nafter",
            Scrubber.ReplaceLines(_ => _ == "a" ? "x\ny" : _),
            Scrubber.ReplaceLines(_ => _ == "x" ? "z" : _));
        Assert.Equal("before\nz\ny\nafter", result);
    }
}
