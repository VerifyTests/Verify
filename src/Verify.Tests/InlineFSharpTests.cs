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

    // Built on demand by WriteTemplate, so the tests that never write one neither create a
    // directory nor put a path in front of the temp path scrubber
    TempDirectory? templates;

    public InlineFSharpTests() =>
        InlineEngine.IsBuildServer = () => false;

    public void Dispose()
    {
        InlineEngine.IsBuildServer = originalIsBuildServer;
        templates?.Dispose();
    }

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

    /// <summary>
    /// The agreement Verify's F# comparison rests on, asserted directly rather than only through
    /// a verification.
    /// <para>
    /// It is not Verify's to keep: taking the layout off is DiffEngine's
    /// <c>SourceLanguage.SnapshotValue</c>, reached through <c>NormalizeExpected</c>, and the
    /// package it comes from moves on its own. A release that changed it would flip the pass or
    /// fail of every F# inline snapshot with no change here, and the failures would say the
    /// snapshot differed rather than that the contract had. This says which.
    /// </para>
    /// </summary>
    [Fact]
    public void FSharpLayoutIsTakenOffByTheDependency()
    {
        // What the compiler hands over, into what the comparison uses
        Assert.Equal("line one\nline two", InlineEngine.NormalizeExpected(asFSharpHandsItOver, "Tests.fs"));

        // A value not written to that shape is its own content, which is what keeps a single line
        // snapshot, or one that merely looks like layout, from being trimmed into something else
        Assert.Equal("line one", InlineEngine.NormalizeExpected("line one", "Tests.fs"));

        // C# has raw strings, so its compiler has already done it and there is nothing to take off
        Assert.Equal("line one\nline two", InlineEngine.NormalizeExpected("line one\r\nline two", "Tests.cs"));
    }

    // begin-snippet: InlineFSharpMatches
    [Fact]
    public Task LayoutIsNotContent()
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.Snapshot(asFSharpHandsItOver, FakeSource(), 1, null);
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
        settings.Snapshot(asFSharpHandsItOver, FakeCsSource(), 1, null);

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
        settings.Snapshot("\n        line one\n\n        ", FakeSource(), 1, null);
        return Verify("line one\n", settings);
    }

    // A single line snapshot is a regular literal, which has no layout to take off
    [Fact]
    public Task SingleLine()
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        settings.Snapshot("the value", FakeSource(), 1, null);
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
        settings.Snapshot(RenderedAsFSharp(content), FakeSource(), 1, null);
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

    // xunit builds an instance per test, so the one template name cannot collide
    string WriteTemplate(string body)
    {
        templates ??= new();
        var path = templates.BuildPath("Template.fs");
        File.WriteAllText(path, body);
        return path;
    }
}
