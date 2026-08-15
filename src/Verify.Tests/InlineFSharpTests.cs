// The F# half of inline snapshots. F# has no raw string, so a triple-quoted literal hands over the
// line break after the opening delimiter and the indentation of every line, and the snapshot is
// what is left once DiffEngine takes that layout back off. These tests are that agreement: what
// the compiler would hand over goes in, and the snapshot has to come out.
// ReSharper disable ConstantExpected
[SuppressMessage("Performance", "CA1857:A constant is expected for the parameter")]
// IsBuildServer below is global, so a class that swaps it cannot run beside another that does:
// the first to finish restores the real detector under the one still running, and its accepts
// then decline to rewrite anything. Shared with InlineTests, which swaps the same field
[Collection("Inline")]
public class InlineFSharpTests :
    IDisposable
{
    // Accepting rewrites source, which InlineEngine declines to do on a build server
    Func<bool> originalIsBuildServer = InlineEngine.IsBuildServer;

    public InlineFSharpTests() =>
        InlineEngine.IsBuildServer = () => false;

    public void Dispose() =>
        InlineEngine.IsBuildServer = originalIsBuildServer;

    // What the F# compiler produces for
    //
    //     .Snapshot(
    //         """
    //         line one
    //         line two
    //         """)
    //
    // is everything between the delimiters, verbatim
    const string asFSharpHandsItOver = "\n        line one\n        line two\n        ";

    // begin-snippet: InlineFSharpMatches
    [Fact]
    public Task LayoutIsNotContent()
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.Snapshot(asFSharpHandsItOver, FakeSource(), 1, null, "LayoutIsNotContent");
        return Verify("line one\nline two", settings);
    }
    // end-snippet

    // The same value from a C# file is the snapshot as it stands, because the C# compiler already
    // took the layout off. Reading it the F# way there would silently eat a snapshot's indentation
    [Fact]
    public async Task LayoutIsContentInCSharp()
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.DisableDiff();
        settings.Snapshot(asFSharpHandsItOver, FakeCsSource(), 1, null, "LayoutIsContentInCSharp");

        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await Verify("line one\nline two", settings));

        Assert.Contains("InlineNotEqual", exception.Message);
    }

    // Content ending in a newline is a blank line before the closing delimiter
    [Fact]
    public Task TrailingNewline()
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.Snapshot("\n        line one\n\n        ", FakeSource(), 1, null, "TrailingNewline");
        return Verify("line one\n", settings);
    }

    // A single line snapshot is a regular literal, which has no layout to take off
    [Fact]
    public Task SingleLine()
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.Snapshot("the value", FakeSource(), 1, null, "SingleLine");
        return Verify("the value", settings);
    }

    // A snapshot that happens to look like layout is still the snapshot: it round trips, because
    // what was rendered for it is not the shape the trim reads
    [Fact]
    public Task ContentThatLooksLikeLayout()
    {
        var content = "\n    indented\n    ";
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.Snapshot(RenderedAsFSharp(content), FakeSource(), 1, null, "ContentThatLooksLikeLayout");
        return Verify(content, settings);
    }

    // begin-snippet: InlineFSharpAccept
    [Fact]
    public async Task AcceptWritesTheIndentedForm()
    {
        var template = WriteTemplate(
            """
            module Tests

            let MyTest () =
                Verifier.Verify(value).Snapshot("old").ToTask()
            """);
        try
        {
            var settings = new VerifySettings();
            settings.IgnoreParameters();
            settings.Snapshot("old", template, 4, null, "MyTest");
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("line one\nline two", settings);

            Assert.Equal(
                """"
                module Tests

                let MyTest () =
                    Verifier.Verify(value).Snapshot(
                        """
                        line one
                        line two
                        """).ToTask()
                """",
                await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }
    // end-snippet

    // F# does not implement CallerArgumentExpression, so the patch carries the previous value
    // instead. Two tests with the same snapshot and a hint pointing at the wrong one: the value
    // and the member name are what land the rewrite in the right test
    [Fact]
    public async Task AcceptFindsTheCallWithNoExpression()
    {
        var template = WriteTemplate(
            """
            module Tests

            let TestA () =
                Verifier.Verify(a).Snapshot("dup").ToTask()

            let TestB () =
                Verifier.Verify(b).Snapshot("dup").ToTask()
            """);
        try
        {
            var settings = new VerifySettings();
            settings.IgnoreParameters();
            // The hint is stale and points at TestA
            settings.Snapshot("dup", template, 4, null, "TestB");
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("changed", settings);

            var content = await File.ReadAllTextAsync(template);
            Assert.Contains("Verify(a).Snapshot(\"dup\")", content);
            Assert.Contains("Verify(b).Snapshot(\"changed\")", content);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    // Accepting a new snapshot writes the Snapshot argument the same way
    [Fact]
    public async Task AcceptInsertsIntoAnEmptyCall()
    {
        var template = WriteTemplate(
            """
            module Tests

            let MyTest () =
                Verifier.Verify(value).Snapshot().ToTask()
            """);
        try
        {
            var settings = new VerifySettings();
            settings.IgnoreParameters();
            settings.Snapshot(null, template, 4, null, "MyTest");
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("line one\nline two", settings);

            Assert.Contains(
                """"
                    Verifier.Verify(value).Snapshot(
                        """
                        line one
                        line two
                        """).ToTask()
                """",
                await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    // What DiffEngine writes for this content, minus the delimiters: the value F# would hand over
    static string RenderedAsFSharp(string content)
    {
        var rendered = FsStringLiteral.Render(content, "        ", "\n");
        return rendered.StartsWith("\"\"\"", StringComparison.Ordinal)
            ? rendered.Substring(3, rendered.Length - 6)
            : content;
    }

    // Deliberately not a real source file: a failing inline verify stages a patch, and pointing it
    // at real source would let a tray accept rewrite it
    static string FakeSource() =>
        Path.Combine(Path.GetTempPath(), "VerifyInlineFakeSource.fs");

    static string FakeCsSource() =>
        Path.Combine(Path.GetTempPath(), "VerifyInlineFakeSource.cs");

    static string WriteTemplate(string body)
    {
        var directory = Path.Combine(Path.GetTempPath(), $"VerifyInlineFSharpTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "Template.fs");
        File.WriteAllText(path, body);
        return path;
    }
}
