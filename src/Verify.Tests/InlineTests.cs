// ReSharper disable ConstantExpected
[SuppressMessage("Performance", "CA1857:A constant is expected for the parameter")]
public class InlineTests
{
    [Fact]
    public async Task Simple()
    {
        var result = await Verify("value")
            .Snapshot("value");
        Assert.Equal("value", result.Text);
    }

    #region InlineSample

    [Fact]
    public Task MultiLine()
    {
        var input = "line1\nline2";
        return Verify(input)
            .Snapshot(
                """
                line1
                line2
                """);
    }

    #endregion

    [Fact]
    public Task EmptyString() =>
        Verify("")
            .Snapshot("emptyString");

    [Fact]
    public Task Object() =>
        Verify(
                new
                {
                    a = 1
                })
            .Snapshot(
                """
                {
                  a: 1
                }
                """);

    /// <summary>
    /// The terminator composes with every entry point, which is the point of it not being a
    /// dedicated VerifyInline overload.
    /// </summary>
    [Fact]
    public Task Xml() =>
        VerifyXml("<a><b/></a>")
            .Snapshot(
                """
                <a>
                  <b />
                </a>
                """);

    [Fact]
    public Task Json() =>
        VerifyJson("{a: 1}")
            .Snapshot(
                """
                {
                  a: 1
                }
                """);

    [Fact]
    public Task EdgeContent() =>
        Verify("has \"\"\" quotes\n\nand a blank line")
            .Snapshot(
                """"
                has """ quotes

                and a blank line
                """");

    [Fact]
    public Task CrlfExpectedMatchesLfReceived()
    {
        var settings = new VerifySettings();
        settings.Snapshot("line1\r\nline2", FakeSource(), 1, "\"ignored\"");
        return Verify("line1\nline2", settings);
    }

    [Fact]
    public async Task MultipleInlinePerMethod()
    {
        await Verify("first")
            .Snapshot("first");
        await Verify("second")
            .Snapshot("second");
    }

    /// <summary>
    /// Only the first target is inlined. The rest keep the names they would have had without
    /// inline, so the #01 file below is the same file it would be with no literal at all.
    /// </summary>
    [Fact]
    public Task MultiTarget() =>
        Verify("root")
            .AppendContentAsFile("extra")
            .Snapshot("root");

    [Fact]
    public async Task NotInlineBeatsAnExplicitSnapshot()
    {
        var settings = new VerifySettings();
        settings.NotInline();
        settings.Snapshot("ignored", FakeSource(), 1, "\"ignored\"");

        var result = await Verify("value", settings);

        // A file pair, not an inline result
        Assert.NotEmpty(result.Files);
    }

    [Fact]
    public async Task MismatchThrows()
    {
        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await Verify("value")
                .DisableDiff()
                .Snapshot("wrong"));
        Assert.Contains("InlineNotEqual:", exception.Message);
        Assert.Contains("Source: ", exception.Message);
        Assert.Contains("Received:", exception.Message);
        Assert.Contains("Expected:", exception.Message);
        Assert.Contains("value", exception.Message);
    }

    [Fact]
    public async Task NewThrows()
    {
        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await Verify("value")
                .DisableDiff()
                .Snapshot());
        Assert.Contains("InlineNew:", exception.Message);
        Assert.Contains("Received:", exception.Message);
        // The content block has no Expected section for a new snapshot
        Assert.DoesNotContain("\nExpected:\n", exception.Message);
    }

    [Fact]
    public async Task FirstTargetMustBeText()
    {
        var settings = new VerifySettings();
        settings.Snapshot("ignored", FakeSource(), 1, "\"ignored\"");

        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await Verify(new MemoryStream([1, 2, 3]), "bin", settings));

        Assert.Contains("only support text", exception.Message);
        Assert.Contains("NotInline", exception.Message);
        Assert.Contains("DontInline", exception.Message);
    }

    [Fact]
    public void NonCsFile()
    {
        var settings = new VerifySettings();
        var exception = Assert.ThrowsAny<Exception>(
            () => settings.Snapshot("x", "Tests.vb", 1, "\"x\""));
        Assert.Contains("C# source files", exception.Message);
    }

    // Deliberately not a real source file: a failing inline verify stages a patch, and
    // pointing it at real source would let a tray accept rewrite it
    static string FakeSource() =>
        Path.Combine(Path.GetTempPath(), "VerifyInlineFakeSource.cs");

    static string WriteTemplate(string body)
    {
        var directory = Path.Combine(Path.GetTempPath(), $"VerifyInlineTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "Template.cs");
        File.WriteAllText(path, body);
        return path;
    }

    // The template's line endings would otherwise be whatever the checkout produced
    static string WriteTemplate(string body, string eol) =>
        WriteTemplate(
            body
                .Replace("\r\n", "\n")
                .Replace("\n", eol));

    // Every line ending in the rewritten file must match the original, with no strays
    static void AssertEolConsistent(string text, string eol)
    {
        for (var index = 0; index < text.Length; index++)
        {
            var current = text[index];
            if (current == '\r')
            {
                Assert.Equal("\r\n", eol);
                Assert.True(index + 1 < text.Length && text[index + 1] == '\n');
            }
            else if (current == '\n' &&
                     eol == "\r\n")
            {
                Assert.True(index > 0 && text[index - 1] == '\r');
            }
        }
    }

    // The literal inherits the .cs file's line endings, so the expected value can arrive
    // with any of these and must still match the \n normalized received text
    [Theory]
    [InlineData("\r\n")]
    [InlineData("\r")]
    [InlineData("\n")]
    public Task ExpectedLiteralEolNormalized(string eol)
    {
        var settings = new VerifySettings();
        settings.Snapshot($"line1{eol}line2", FakeSource(), 1, "\"ignored\"");
        return Verify("line1\nline2", settings);
    }

    // Scrubbing normalizes received content, so a target containing CR still matches
    // an expected literal written with \n
    [Theory]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public Task ReceivedContentEolNormalized(string eol) =>
        Verify($"line1{eol}line2")
            .Snapshot(
                """
                line1
                line2
                """);

    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    public async Task AutoVerifyRewriteHonorsTemplateEol(string eol)
    {
        var template = WriteTemplate(mismatchTemplate, eol);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("new1\nnew2", settings);

            var content = await File.ReadAllTextAsync(template);
            var indent = new string(' ', 12);
            Assert.Contains(
                $".Snapshot(\"\"\"{eol}{indent}new1{eol}{indent}new2{eol}{indent}\"\"\");",
                content);
            AssertEolConsistent(content, eol);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    public async Task AutoVerifyInsertHonorsTemplateEol(string eol)
    {
        var template = WriteTemplate(insertTemplate, eol);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot(null, template, 4, null);
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("new1\nnew2", settings);

            var content = await File.ReadAllTextAsync(template);
            var indent = new string(' ', 12);
            Assert.Contains(
                $".Snapshot(\"\"\"{eol}{indent}new1{eol}{indent}new2{eol}{indent}\"\"\");",
                content);
            AssertEolConsistent(content, eol);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    const string mismatchTemplate =
        """
        class Templ
        {
            Task Method() =>
                Verify(value).Snapshot("old");
        }
        """;

    const string insertTemplate =
        """
        class Templ
        {
            Task Method() =>
                Verify(value).Snapshot();
        }
        """;

    [Fact]
    public async Task AutoVerifyMismatchRewritesSource()
    {
        var template = WriteTemplate(mismatchTemplate);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            var content = await File.ReadAllTextAsync(template);
            Assert.Contains("newvalue", content);
            Assert.DoesNotContain("\"old\"", content);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task AutoVerifyNewInsertsLiteral()
    {
        var template = WriteTemplate(insertTemplate);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot(null, template, 4, null);
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            Assert.Contains(".Snapshot(\"\"\"", await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task AutoVerifyAlreadyApplied()
    {
        // The literal already matches (eg the other target framework accepted first),
        // but the captured expression is stale
        var template = WriteTemplate(
            """
            class Templ
            {
                Task Method() =>
                    Verify(value).Snapshot("newvalue");
            }
            """);
        try
        {
            var before = await File.ReadAllTextAsync(template);
            var settings = new VerifySettings();
            settings.Snapshot("stale", template, 4, "\"stale\"");
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            Assert.Equal(before, await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task AutoVerifyNotFoundFallsBackToThrow()
    {
        var template = WriteTemplate(
            """
            class Templ
            {
                Task Method() =>
                    Verify(value).Snapshot("different");
            }
            """);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("stale", template, 4, "\"stale\"");
            settings.AutoVerify();
            settings.DisableDiff();
            var exception = await Assert.ThrowsAsync<VerifyException>(
                async () => await Verify("newvalue", settings));
            Assert.Contains("InlineNotEqual:", exception.Message);
            Assert.Contains("\"different\"", await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    static int Count(string text, string value)
    {
        var count = 0;
        var index = 0;
        while (true)
        {
            index = text.IndexOf(value, index, StringComparison.Ordinal);
            if (index < 0)
            {
                return count;
            }

            count++;
            index += value.Length;
        }
    }

    const string twoSiteTemplate =
        """
        class Templ
        {
            Task A() => Verify(a).Snapshot("old");
            Task B() => Verify(b).Snapshot("old");
        }
        """;

    static VerifySettings AcceptSettings(string template, int line, string expression)
    {
        var settings = new VerifySettings();
        settings.Snapshot("old", template, line, expression);
        settings.AutoVerify();
        settings.DisableDiff();
        return settings;
    }

    // Two tests in the same file producing the same result. The identical literals give
    // the search nothing to tell the sites apart beyond the line hint, so both must still
    // be patched rather than one being taken twice.
    [Fact]
    public async Task TwoSitesInSameFileWithSameResult()
    {
        var template = WriteTemplate(twoSiteTemplate);
        try
        {
            await Verify("same", AcceptSettings(template, 3, "\"old\""));
            await Verify("same", AcceptSettings(template, 4, "\"old\""));

            var content = await File.ReadAllTextAsync(template);
            Assert.DoesNotContain("\"old\"", content);
            Assert.Equal(2, Count(content, "same"));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task TwoSitesInSameFileWithDifferentResults()
    {
        var template = WriteTemplate(twoSiteTemplate);
        try
        {
            await Verify("resultA", AcceptSettings(template, 3, "\"old\""));
            await Verify("resultB", AcceptSettings(template, 4, "\"old\""));

            var content = await File.ReadAllTextAsync(template);
            Assert.DoesNotContain("\"old\"", content);
            var indexA = content.IndexOf("Verify(a)", StringComparison.Ordinal);
            var indexB = content.IndexOf("Verify(b)", StringComparison.Ordinal);
            var segmentA = content.Substring(indexA, indexB - indexA);
            // Each site keeps its own result
            Assert.Contains("resultA", segmentA);
            Assert.DoesNotContain("resultB", segmentA);
            Assert.Contains("resultB", content.Substring(indexB));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task ParallelSameFileAcceptsWithIdenticalLiterals()
    {
        var template = WriteTemplate(twoSiteTemplate);
        try
        {
            await Task.WhenAll(
                Verify("same", AcceptSettings(template, 3, "\"old\"")),
                Verify("same", AcceptSettings(template, 4, "\"old\"")));

            var content = await File.ReadAllTextAsync(template);
            Assert.DoesNotContain("\"old\"", content);
            Assert.Equal(2, Count(content, "same"));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task ParallelSameFileAccepts()
    {
        var template = WriteTemplate(
            """
            class Templ
            {
                Task A() => Verify(a).Snapshot("oldA");
                Task B() => Verify(b).Snapshot("oldB");
                Task C() => Verify(c).Snapshot("oldC");
            }
            """);
        try
        {
            async Task Accept(string old, int line, string value)
            {
                var settings = new VerifySettings();
                settings.Snapshot(old, template, line, $"\"{old}\"");
                settings.AutoVerify();
                settings.DisableDiff();
                await Verify(value, settings);
            }

            await Task.WhenAll(
                Accept("oldA", 3, "newA"),
                Accept("oldB", 4, "newB"),
                Accept("oldC", 5, "newC"));

            var content = await File.ReadAllTextAsync(template);
            Assert.Contains("newA", content);
            Assert.Contains("newB", content);
            Assert.Contains("newC", content);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    /// <summary>
    /// The inlined target never claims its verified file, so the file a previous run left behind
    /// flows through the normal delete path.
    /// </summary>
    [Fact]
    public async Task MovedToInlineCleanup()
    {
        var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "InlineScratch");
        Directory.CreateDirectory(directory);
        var stale = Path.Combine(directory, "MovedToInline.verified.txt");
        await File.WriteAllTextAsync(stale, "stale");
        try
        {
            var settings = new VerifySettings();
            settings.UseDirectory("InlineScratch");
            settings.UseFileName("MovedToInline");
            settings.AutoVerify();
            settings.DisableDiff();
            settings.Snapshot("value", FakeSource(), 1, "\"value\"");
            await Verify("value", settings);
            Assert.False(File.Exists(stale));
        }
        finally
        {
            if (File.Exists(stale))
            {
                File.Delete(stale);
            }
        }
    }

    /// <summary>
    /// The other direction: a test with a literal that opts out. The Snapshot call is stripped and
    /// the snapshot goes back to being a file, which the user accepts the usual way.
    /// </summary>
    [Fact]
    public async Task MovedToFileRemovesTheLiteral()
    {
        var template = WriteTemplate(mismatchTemplate);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.NotInline();
            settings.AutoVerify();
            settings.DisableDiff();
            settings.UseDirectory("InlineScratch");
            settings.UseFileName("MovedToFile");

            await Verify("value", settings);

            Assert.DoesNotContain("Snapshot", await File.ReadAllTextAsync(template));
            var verified = Path.Combine(AttributeReader.GetProjectDirectory(), "InlineScratch", "MovedToFile.verified.txt");
            Assert.True(File.Exists(verified));
            File.Delete(verified);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task BuildServerDoesNotRewrite()
    {
        var template = WriteTemplate(mismatchTemplate);
        var original = await File.ReadAllTextAsync(template);
        BuildServerDetector.Detected = true;
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();
            await Assert.ThrowsAsync<VerifyException>(
                async () => await Verify("newvalue", settings));
            Assert.Equal(original, await File.ReadAllTextAsync(template));
        }
        finally
        {
            BuildServerDetector.Detected = false;
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task BuildServerDoesNotRemoveTheLiteral()
    {
        var template = WriteTemplate(mismatchTemplate);
        var original = await File.ReadAllTextAsync(template);
        BuildServerDetector.Detected = true;
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.NotInline();
            settings.AutoVerify();
            settings.DisableDiff();
            settings.UseDirectory("InlineScratch");
            settings.UseFileName("BuildServerNotInline");

            await Verify("value", settings);

            Assert.Equal(original, await File.ReadAllTextAsync(template));
            var verified = Path.Combine(AttributeReader.GetProjectDirectory(), "InlineScratch", "BuildServerNotInline.verified.txt");
            if (File.Exists(verified))
            {
                File.Delete(verified);
            }
        }
        finally
        {
            BuildServerDetector.Detected = false;
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }
}
