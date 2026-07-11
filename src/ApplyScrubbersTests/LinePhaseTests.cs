public class LinePhaseTests
{
    [Fact]
    public void Transforms_ComposeInRegistrationOrder()
    {
        var result = EngineRunner.Run(
            "a",
            Scrubber.ReplaceLines(_ => _ == "a" ? "b" : _),
            Scrubber.ReplaceLines(_ => _ == "b" ? "c" : _));
        Assert.Equal("c", result);
    }

    [Fact]
    public void Transform_LineResultVariants()
    {
        var result = EngineRunner.Run(
            "keep\nremove\nreplace",
            Scrubber.ReplaceLines(line =>
            {
                if (line.SequenceEqual("remove".AsSpan()))
                {
                    return LineResult.Remove;
                }

                if (line.SequenceEqual("replace".AsSpan()))
                {
                    return LineResult.Replace("replaced");
                }

                return LineResult.Keep;
            }));
        Assert.Equal("keep\nreplaced", result);
    }

    [Fact]
    public void Transform_NullDrops()
    {
        var result = EngineRunner.Run(
            "keep\ndrop me",
            Scrubber.ReplaceLines(_ => _.Contains("drop") ? null : _));
        Assert.Equal("keep", result);
    }

    [Fact]
    public void TransformedLine_RescannedByInlineScrubbers()
    {
        var result = EngineRunner.Run(
            "line",
            Scrubber.ReplaceLines(_ => _ == "line" ? "token here" : _),
            Scrubber.Replace("token", "X"));
        Assert.Equal("X here", result);
    }

    [Fact]
    public void Drops_SeeRawLines_NotTransformOutput()
    {
        // The transform would rewrite the marker, but drops run first on the raw line
        var result = EngineRunner.Run(
            "clean\nsecret data",
            Scrubber.ReplaceLines(_ => _.Replace("secret", "public")),
            Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "secret"));
        Assert.Equal("clean", result);
    }

    [Fact]
    public void NeedleDrop_LengthSkip()
    {
        // Lines shorter than the needle cannot match and are kept
        var result = EngineRunner.Run(
            "ab\nlongneedle here\ncd",
            Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "longneedle"));
        Assert.Equal("ab\ncd", result);
    }

    [Fact]
    public void ComparisonBucketing()
    {
        var result = EngineRunner.Run(
            "foo\nFOO\nbar\nBAR\nkeep",
            Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "foo"),
            Scrubber.RemoveLinesContaining("BAR"));
        Assert.Equal("FOO\nkeep", result);
    }

    [Fact]
    public void RemoveEmptyLines_DropsWhitespaceLines()
    {
        var result = EngineRunner.Run(
            "a\n\n  \t\nb",
            Scrubber.RemoveEmptyLines());
        Assert.Equal("a\nb", result);
    }

    [Fact]
    public void RemoveEmptyLines_TrimsTrailingNewline()
    {
        // Matches legacy RemoveEmptyLines: the trailing newline is trimmed even when
        // no line was dropped
        var result = EngineRunner.Run(
            "a\nb\n",
            Scrubber.RemoveEmptyLines());
        Assert.Equal("a\nb", result);
    }

    [Fact]
    public void SpanPredicateOverload()
    {
        var result = EngineRunner.Run(
            "keep\nremove",
            Scrubber.RemoveLines((LineMatch) (line => line.StartsWith("remove".AsSpan()))));
        Assert.Equal("keep", result);
    }

    [Fact]
    public void UntypedLambda_BindsStringOverload()
    {
        // _.Contains('D') is valid for both string and span, so overload
        // resolution priority must pick the string overload instead of
        // reporting an ambiguity
        var result = EngineRunner.Run(
            "a\nD\nb",
            Scrubber.RemoveLines(_ => _.Contains('D')));
        Assert.Equal("a\nb", result);
    }

    [Fact]
    public void SpanSugarOverloads()
    {
        var settings = new VerifySettings();
        // Explicitly typed span lambdas select the LineMatch/LineReplace overloads
        settings.ScrubLines((CharSpan line) => line.StartsWith("remove".AsSpan()));
        settings.ScrubLinesWithReplace((CharSpan line) =>
        {
            if (line.StartsWith("replace".AsSpan()))
            {
                return LineResult.Replace("replaced");
            }

            return LineResult.Keep;
        });
        // Untyped lambda still binds the string overload without ambiguity
        settings.ScrubLines(_ => _.Contains("drop"));

        using var counter = Counter.Start();
        var builder = new StringBuilder("keep\nremove\nreplace me\ndrop me");
        ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
        Assert.Equal("keep\nreplaced", builder.ToString());
    }

    [Fact]
    public void DropFirstMiddleLast()
    {
        Assert.Equal("b\nc", EngineRunner.Run("x\nb\nc", Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "x")));
        Assert.Equal("a\nc", EngineRunner.Run("a\nx\nc", Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "x")));
        Assert.Equal("a\nb", EngineRunner.Run("a\nb\nx", Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "x")));
        Assert.Equal("", EngineRunner.Run("x\nx\nx", Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "x")));
    }

    [Fact]
    public void TrailingNewlinePreservedOnDrop()
    {
        Assert.Equal("a\n", EngineRunner.Run("a\nx\n", Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "x")));
        Assert.Equal("a", EngineRunner.Run("a\nx", Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "x")));
    }

    [Fact]
    public void LineResultReplace_NormalizesNewlines()
    {
        var replaced = LineResult.Replace("a\r\nb");
        var result = EngineRunner.Run(
            "line",
            Scrubber.ReplaceLines(_ => replaced));
        Assert.Equal("a\nb", result);
    }
}
