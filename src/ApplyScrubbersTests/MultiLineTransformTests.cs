// A line transform is defined over a single line, so when one produces line
// breaks the remaining transforms must see each produced line on its own.
public class MultiLineTransformTests
{
    [Fact]
    public void LaterTransformSeesProducedLines()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines((string line) => line == "a" ? "x\ny" : line),
            Scrubber.ReplaceLines((string line) => line == "x" ? "z" : line));
        Assert.Equal("z\ny", result);
    }

    [Fact]
    public void LaterTransformSeesEveryProducedLine()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines((string line) => line == "a" ? "x\ny\nz" : line),
            Scrubber.ReplaceLines((string line) => line.ToUpperInvariant()));
        Assert.Equal("X\nY\nZ", result);
    }

    [Fact]
    public void LaterTransformCanRemoveAProducedLine()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines((string line) => line == "a" ? "keep\ndrop\nkeep2" : line),
            Scrubber.ReplaceLines((string line) => line == "drop" ? null : line));
        Assert.Equal("keep\nkeep2", result);
    }

    [Fact]
    public void RemovingEveryProducedLineRemovesTheLine()
    {
        var result = EngineRunner.Run(
            "a\nb",
            Scrubber.ReplaceLines((string line) => line == "a" ? "x\ny" : line),
            Scrubber.ReplaceLines((string line) => line is "x" or "y" ? null : line));
        Assert.Equal("b", result);
    }

    [Fact]
    public void ProducedLinesCanExpandAgain()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines((string line) => line == "a" ? "x\ny" : line),
            Scrubber.ReplaceLines((string line) => line == "y" ? "1\n2" : line),
            Scrubber.ReplaceLines((string line) => line == "2" ? "end" : line));
        Assert.Equal("x\n1\nend", result);
    }

    [Fact]
    public void SpanTransformSeesProducedLines()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines((CharSpan line) => line.SequenceEqual("a".AsSpan()) ? LineResult.Replace("x\ny") : LineResult.Keep),
            Scrubber.ReplaceLines((CharSpan line) => line.SequenceEqual("x".AsSpan()) ? LineResult.Replace("z") : LineResult.Keep));
        Assert.Equal("z\ny", result);
    }

    [Fact]
    public void TrailingTransformLeavesTextExactlyAsProduced()
    {
        // Nothing follows the expanding transform, so the text is untouched
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines((string line) => line == "a" ? "x\ny\n" : line));
        Assert.Equal("x\ny\n", result);
    }

    [Fact]
    public void SurroundingLinesArePreserved()
    {
        var result = EngineRunner.Run(
            "before\na\nafter",
            Scrubber.ReplaceLines((string line) => line == "a" ? "x\ny" : line),
            Scrubber.ReplaceLines((string line) => line == "x" ? "z" : line));
        Assert.Equal("before\nz\ny\nafter", result);
    }
}
