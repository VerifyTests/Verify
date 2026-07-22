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
    public Task ScrubAllLinesToEmpty()
    {
        // Scrubbing away every line leaves empty content. It is normalized to the
        // "emptyString" sentinel (the same one used for Verify(string.Empty)) so that
        // empty content is never written as a snapshot nor passed to a string comparer.
        var settings = new VerifySettings();
        settings.ScrubLines(removeLine: _ => true);
        return Verify(
            settings: settings,
            target: """
                    a
                    b
                    c
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

    static Dictionary<string, object> emptyContext = [];

    static string Run(string input, Scrubber scrubber)
    {
        using var counter = Counter.Start();
        var set = EngineScrubberSet.ForScrubbers([scrubber]);
        return ScrubEngine.Run(input, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    static string Filter(string input, Func<string, bool> removeLine) =>
        Run(input, Scrubber.RemoveLines(removeLine));

    static string Replace(string input, Func<string, string?> replaceLine) =>
        Run(input, Scrubber.ReplaceLines(replaceLine));

    [Fact]
    public void ReplaceLines_AllRemovedNoTrailingNewline() =>
        // Single line, no trailing newline, and every line replaced with null
        Assert.Equal(string.Empty, Replace("single line", _ => null));

    [Fact]
    public void ReplaceLines_ReplacesLines() =>
        Assert.Equal("a\nB\nc", Replace("a\nb\nc", _ => _ == "b" ? "B" : _));

    [Fact]
    public void FilterLines_RemovesSingleLine() =>
        Assert.Equal("line1\nline3", Filter("line1\nline2\nline3", _ => _ == "line2"));

    [Fact]
    public void FilterLines_RemovesMultipleLines() =>
        Assert.Equal("keep1\nkeep2\nkeep3", Filter("keep1\nremove1\nkeep2\nremove2\nkeep3", _ => _.StartsWith("remove")));

    [Fact]
    public void FilterLines_RemovesAllLines() =>
        Assert.Equal(string.Empty, Filter("line1\nline2\nline3", _ => true));

    [Fact]
    public void FilterLines_RemovesNoLines() =>
        Assert.Equal("line1\nline2\nline3", Filter("line1\nline2\nline3", _ => false));

    [Fact]
    public void FilterLines_EmptyString() =>
        Assert.Equal(string.Empty, Filter(string.Empty, _ => true));

    [Fact]
    public void FilterLines_SingleLineNoNewline() =>
        Assert.Equal("single line", Filter("single line", _ => false));

    [Fact]
    public void FilterLines_SingleLineWithNewline() =>
        Assert.Equal("single line\n", Filter("single line\n", _ => false));

    [Fact]
    public void FilterLines_PreservesTrailingNewline() =>
        Assert.Equal("line1\n", Filter("line1\nline2\n", _ => _ == "line2"));

    [Fact]
    public void FilterLines_DoesNotAddTrailingNewlineWhenOriginalLacked() =>
        Assert.Equal("line2", Filter("line1\nline2", _ => _ == "line1"));

    [Fact]
    public void FilterLines_HandlesEmptyLines() =>
        Assert.Equal("line1\nline3", Filter("line1\n\nline3", string.IsNullOrEmpty));

    [Fact]
    public void FilterLines_KeepsEmptyLines() =>
        Assert.Equal("\nline3", Filter("line1\n\nline3", _ => _ == "line1"));

    [Fact]
    public void FilterLines_OnlyEmptyLines() =>
        Assert.Equal("\n\n", Filter("\n\n", _ => false));

    [Fact]
    public void FilterLines_RemovesFirstLine() =>
        Assert.Equal("keep1\nkeep2", Filter("remove\nkeep1\nkeep2", _ => _ == "remove"));

    [Fact]
    public void FilterLines_RemovesLastLine() =>
        Assert.Equal("keep1\nkeep2", Filter("keep1\nkeep2\nremove", _ => _ == "remove"));

    [Fact]
    public void FilterLines_RemovesLastLineWithTrailingNewline() =>
        Assert.Equal("keep1\nkeep2\n", Filter("keep1\nkeep2\nremove\n", _ => _ == "remove"));

    [Fact]
    public void FilterLines_ComplexPredicate() =>
        Assert.Equal("abc123\nghi789\njkl012", Filter("abc123\ndef456\nghi789\njkl012", _ => _.Any(char.IsDigit) && _.Contains('4')));

    [Fact]
    public void FilterLines_WindowsLineEndings() =>
        // \r\n is normalized to \n
        Assert.Equal("line1\nline3", Filter("line1\r\nline2\r\nline3", _ => _ == "line2"));

    [Fact]
    public void FilterLines_MixedLineEndings() =>
        Assert.Equal("line1\nline3", Filter("line1\nline2\r\nline3", _ => _ == "line2"));

    [Fact]
    public void FilterLines_LargeContent()
    {
        var lines = Enumerable.Range(1, 1000).Select(_ => $"line{_}");
        var input = string.Join('\n', lines);

        var result = Filter(input, _ => int.Parse(_[4..]) % 2 == 0);

        var remaining = result.Split('\n');
        Assert.Equal(500, remaining.Length);
        Assert.All(remaining, _ => Assert.True(int.Parse(_[4..]) % 2 == 1));
    }

    [Fact]
    public void FilterLines_PreservesWhitespace() =>
        Assert.Equal("  line1  \nline3   ", Filter("  line1  \n\t line2\t\nline3   ", _ => _.Trim() == "line2"));

    [Fact]
    public void FilterLines_SingleLineRemoved() =>
        Assert.Equal(string.Empty, Filter("remove this", _ => true));

    [Fact]
    public void FilterLines_AllButLastRemoved() =>
        Assert.Equal("keep\n", Filter("remove1\nremove2\nkeep\n", _ => _.StartsWith("remove")));

    [Fact]
    public void FilterLines_MultipleConsecutiveRemoved() =>
        Assert.Equal("keep1\nkeep2", Filter("keep1\nremove1\nremove2\nremove3\nkeep2", _ => _.StartsWith("remove")));

    [Fact]
    public void FilterLines_VeryLongLine()
    {
        var longLine = new string('a', 10_000);

        var result = Filter($"keep1\n{longLine}\nkeep2", _ => _ == longLine);

        Assert.Equal("keep1\nkeep2", result);
    }

    [Fact]
    public void FilterLines_CrLfInLongContent()
    {
        var lineContent = new string('y', 7999);

        var result = Filter($"{lineContent}\r\nnextline", _ => _ == lineContent);

        Assert.Equal("nextline", result);
    }

    [Fact]
    public void FilterLines_ManyLongLines()
    {
        var builder = new StringBuilder();
        var lines = new List<string>();

        for (var i = 0; i < 20; i++)
        {
            var line = $"line{i}_" + new string((char) ('a' + i % 26), 1000 + i * 100);
            lines.Add(line);
            builder.Append(line);
            if (i < 19)
            {
                builder.Append('\n');
            }
        }

        // Remove even-indexed lines
        var result = Filter(builder.ToString(), _ => lines.IndexOf(_) % 2 == 0);

        var expected = string.Join('\n', lines.Where((_, index) => index % 2 == 1));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FilterLines_VeryLongLineKept()
    {
        var longLine = new string('z', 20_000);

        var result = Filter($"remove\n{longLine}\nremove", _ => _ == "remove");

        Assert.Equal(longLine, result);
    }

    [Fact]
    public void FilterLines_ManyLinesAllRemoved()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < 10; i++)
        {
            builder.Append('a', 2000);
            builder.Append('\n');
        }

        Assert.Equal(string.Empty, Filter(builder.ToString(), _ => true));
    }

    [Fact]
    public void FilterLines_ManyLinesNoneRemoved()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < 10; i++)
        {
            builder.Append((char) ('a' + i), 2000);
            builder.Append('\n');
        }

        var original = builder.ToString();

        Assert.Equal(original, Filter(original, _ => false));
    }

    [Fact]
    public void FilterLines_LongLinePreserved()
    {
        var prefix = new string('p', 7990);
        var suffix = new string('s', 100);
        var fullLine = prefix + suffix;

        var result = Filter($"before\n{fullLine}\nafter", _ => _ == "before");

        Assert.Equal(fullLine + "\nafter", result);
    }

    [Fact]
    public void FilterLines_EmptyLinesAfterLongLine()
    {
        var longLine = new string('x', 8000);

        var result = Filter($"{longLine}\n\n\nkeep", string.IsNullOrEmpty);

        Assert.Equal(longLine + "\nkeep", result);
    }
}
