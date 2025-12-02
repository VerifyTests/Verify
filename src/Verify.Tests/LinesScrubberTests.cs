public class LinesScrubberTests
{
    [Fact]
    public Task ScrubLinesContaining()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesContaining("c", "D", "F");
        return Verify(
            settings: settings,
            target: """
                    a
                    b
                    c
                    D
                    e
                    f
                    """);
    }

    [Fact]
    public Task DontScrubTrailingNewline()
    {
        var settings = new VerifySettings();
        settings.ScrubLines(removeLine: _ => _.Contains('D'));
        return Verify(
            settings: settings,
            target: """
                    b

                    """);
    }

    [Fact]
    public Task DontScrubMultiNewline()
    {
        var settings = new VerifySettings();
        settings.ScrubLines(removeLine: _ => _.Contains('D'));
        return Verify(
            settings: settings,
            target: """
                    b

                    c
                    """);
    }

    [Fact]
    public Task FilterLines()
    {
        var settings = new VerifySettings();
        settings.ScrubLines(removeLine: _ => _.Contains('D'));
        return Verify(
            settings: settings,
            target: """
                    a
                    b
                    c
                    D
                    e
                    f
                    """);
    }

    [Fact]
    public Task ScrubLinesContaining_case_sensitive()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesContaining(StringComparison.Ordinal, "c", "D", "F");
        return Verify(
            settings: settings,
            target: """
                    a
                    b
                    c
                    D
                    e
                    f
                    """);
    }

    [Fact]
    public void FilterLines_RemovesSingleLine()
    {
        var builder = new StringBuilder("line1\nline2\nline3");

        builder.FilterLines(line => line == "line2");

        Assert.Equal("line1\nline3", builder.ToString());
    }

    [Fact]
    public void FilterLines_RemovesMultipleLines()
    {
        var builder = new StringBuilder("keep1\nremove1\nkeep2\nremove2\nkeep3");

        builder.FilterLines(line => line.StartsWith("remove"));

        Assert.Equal("keep1\nkeep2\nkeep3", builder.ToString());
    }

    [Fact]
    public void FilterLines_RemovesAllLines()
    {
        var builder = new StringBuilder("line1\nline2\nline3");

        builder.FilterLines(_ => true);

        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void FilterLines_RemovesNoLines()
    {
        var builder = new StringBuilder("line1\nline2\nline3");
        var original = builder.ToString();

        builder.FilterLines(_ => false);

        Assert.Equal(original, builder.ToString());
    }

    [Fact]
    public void FilterLines_EmptyStringBuilder()
    {
        var builder = new StringBuilder();

        builder.FilterLines(_ => true);

        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void FilterLines_SingleLineNoNewline()
    {
        var builder = new StringBuilder("single line");

        builder.FilterLines(_ => false);

        Assert.Equal("single line", builder.ToString());
    }

    [Fact]
    public void FilterLines_SingleLineWithNewline()
    {
        var builder = new StringBuilder("single line\n");

        builder.FilterLines(_ => false);

        Assert.Equal("single line\n", builder.ToString());
    }

    [Fact]
    public void FilterLines_PreservesTrailingNewline()
    {
        var builder = new StringBuilder("line1\nline2\n");

        builder.FilterLines(line => line == "line2");

        Assert.Equal("line1\n", builder.ToString());
    }

    [Fact]
    public void FilterLines_DoesNotAddTrailingNewlineWhenOriginalLacked()
    {
        var builder = new StringBuilder("line1\nline2");

        builder.FilterLines(line => line == "line1");

        Assert.Equal("line2", builder.ToString());
    }

    [Fact]
    public void FilterLines_HandlesEmptyLines()
    {
        var builder = new StringBuilder("line1\n\nline3");

        builder.FilterLines(string.IsNullOrEmpty);

        Assert.Equal("line1\nline3", builder.ToString());
    }

    [Fact]
    public void FilterLines_KeepsEmptyLines()
    {
        var builder = new StringBuilder("line1\n\nline3");

        builder.FilterLines(line => line == "line1");

        Assert.Equal("\nline3", builder.ToString());
    }

    [Fact]
    public void FilterLines_OnlyEmptyLines()
    {
        var builder = new StringBuilder("\n\n");

        builder.FilterLines(_ => false);

        Assert.Equal("\n\n", builder.ToString());
    }

    [Fact]
    public void FilterLines_RemovesFirstLine()
    {
        var builder = new StringBuilder("remove\nkeep1\nkeep2");

        builder.FilterLines(line => line == "remove");

        Assert.Equal("keep1\nkeep2", builder.ToString());
    }

    [Fact]
    public void FilterLines_RemovesLastLine()
    {
        var builder = new StringBuilder("keep1\nkeep2\nremove");

        builder.FilterLines(line => line == "remove");

        Assert.Equal("keep1\nkeep2", builder.ToString());
    }

    [Fact]
    public void FilterLines_RemovesLastLineWithTrailingNewline()
    {
        var builder = new StringBuilder("keep1\nkeep2\nremove\n");

        builder.FilterLines(line => line == "remove");

        Assert.Equal("keep1\nkeep2\n", builder.ToString());
    }

    [Fact]
    public void FilterLines_ComplexPredicate()
    {
        var builder = new StringBuilder("abc123\ndef456\nghi789\njkl012");

        builder.FilterLines(line => line.Any(char.IsDigit) && line.Contains('4'));

        Assert.Equal("abc123\nghi789\njkl012", builder.ToString());
    }

    [Fact]
    public void FilterLines_WindowsLineEndings()
    {
        var builder = new StringBuilder("line1\r\nline2\r\nline3");

        builder.FilterLines(line => line == "line2");

        // Note: StringReader normalizes \r\n to \n
        Assert.Equal("line1\nline3", builder.ToString());
    }

    [Fact]
    public void FilterLines_MixedLineEndings()
    {
        var builder = new StringBuilder("line1\nline2\r\nline3");

        builder.FilterLines(line => line == "line2");

        Assert.Equal("line1\nline3", builder.ToString());
    }

    [Fact]
    public void FilterLines_LargeContent()
    {
        var lines = Enumerable.Range(1, 1000).Select(i => $"line{i}");
        var builder = new StringBuilder(string.Join('\n', lines));

        builder.FilterLines(line => int.Parse(line[4..]) % 2 == 0);

        var remaining = builder.ToString().Split('\n');
        Assert.Equal(500, remaining.Length);
        Assert.All(remaining, line => Assert.True(int.Parse(line[4..]) % 2 == 1));
    }

    [Fact]
    public void FilterLines_PreservesWhitespace()
    {
        var builder = new StringBuilder("  line1  \n\t line2\t\nline3   ");

        builder.FilterLines(line => line.Trim() == "line2");

        Assert.Equal("  line1  \nline3   ", builder.ToString());
    }

    [Fact]
    public void FilterLines_SingleLineRemoved()
    {
        var builder = new StringBuilder("remove this");

        builder.FilterLines(_ => true);

        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void FilterLines_AllButLastRemoved()
    {
        var builder = new StringBuilder("remove1\nremove2\nkeep\n");

        builder.FilterLines(line => line.StartsWith("remove"));

        Assert.Equal("keep\n", builder.ToString());
    }

    [Fact]
    public void FilterLines_MultipleConsecutiveRemoved()
    {
        var builder = new StringBuilder("keep1\nremove1\nremove2\nremove3\nkeep2");

        builder.FilterLines(line => line.StartsWith("remove"));

        Assert.Equal("keep1\nkeep2", builder.ToString());
    }
}