public class WindowTests
{
    [Fact]
    public void FixedLength()
    {
        var result = EngineRunner.Run(
            "xx ABC xx",
            Scrubber.Window(3, 3, (window, _, _) => window.SequenceEqual("ABC".AsSpan()) ? "match" : null));
        Assert.Equal("xx match xx", result);
    }

    [Fact]
    public void VariableLength_LongestTriedFirst()
    {
        var lengths = new List<int>();
        EngineRunner.Run(
            "abcdefgh",
            Scrubber.Window(2, 4, (window, _, _) =>
            {
                lengths.Add(window.Length);
                return null;
            }));

        // At the first position all lengths fit and are probed longest first
        Assert.Equal([4, 3, 2], lengths.Take(3));
    }

    [Fact]
    public void WindowsNeverContainNewlines()
    {
        var windows = new List<string>();
        EngineRunner.Run(
            "ab\ncd",
            Scrubber.Window(2, 4, (window, _, _) =>
            {
                windows.Add(window.ToString());
                return null;
            }));

        Assert.NotEmpty(windows);
        Assert.All(windows, _ => Assert.DoesNotContain('\n', _));
    }

    [Fact]
    public void WordBoundary_DocumentEdgesAreValid()
    {
        var result = EngineRunner.Run(
            "ABC",
            Scrubber.Window(3, 3, (window, _, _) => window.SequenceEqual("ABC".AsSpan()) ? "match" : null, requireWordBoundary: true));
        Assert.Equal("match", result);
    }

    [Fact]
    public void WordBoundary_AdjacentLetterRejects()
    {
        var result = EngineRunner.Run(
            "xABC ABCx ABC",
            Scrubber.Window(3, 3, (window, _, _) => window.SequenceEqual("ABC".AsSpan()) ? "match" : null, requireWordBoundary: true));
        Assert.Equal("xABC ABCx match", result);
    }

    [Fact]
    public void Counter_FlowsToMatcher()
    {
        using var counter = Counter.Start();
        Counter? seen = null;
        EngineRunner.Run(
            "abc",
            counter,
            Scrubber.Window(3, 3, (_, matcherCounter, _) =>
            {
                seen = matcherCounter;
                return null;
            }));
        Assert.Same(counter, seen);
    }

    [Fact]
    public void GuidWindow_EndToEnd()
    {
        using var counter = Counter.Start();
        var result = EngineRunner.Run(
            "id: 173535ae-995b-4cc6-a74e-8cd4be57039c done",
            counter,
            GuidMatcher.Instance);
        Assert.Equal("id: Guid_1 done", result);
    }
}
