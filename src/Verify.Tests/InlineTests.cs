public class InlineTests
{
    [Fact]
    public async Task Simple()
    {
        var result = await VerifyInline("value", "value");
        Assert.Equal("value", result.Text);
    }

    #region InlineSample

    [Fact]
    public Task MultiLine()
    {
        var input = "line1\nline2";
        return VerifyInline(
            input,
            """
            line1
            line2
            """);
    }

    #endregion

    [Fact]
    public Task EmptyString() =>
        VerifyInline("", "emptyString");

    [Fact]
    public Task Object() =>
        VerifyInline(
            new
            {
                a = 1
            },
            """
            {
              a: 1
            }
            """);

    [Fact]
    public Task EdgeContent() =>
        VerifyInline(
            "has \"\"\" quotes\n\nand a blank line",
            """"
            has """ quotes

            and a blank line
            """");

    [Fact]
    public Task CrlfExpectedMatchesLfReceived()
    {
        var settings = new VerifySettings();
        settings.Inline("line1\r\nline2", FakeSource(), 1, "\"ignored\"");
        return Verify("line1\nline2", settings);
    }

    [Fact]
    public async Task MultipleInlinePerMethod()
    {
        await VerifyInline("first", "first");
        await VerifyInline("second", "second");
    }

    [Fact]
    public Task MultiTarget() =>
        VerifyInline(
            "root",
            """
            ---------- target#00.txt ----------
            root
            ---------- target#01.txt ----------
            extra
            """)
            .AppendContentAsFile("extra");

    [Fact]
    public async Task MismatchThrows()
    {
        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await VerifyInline("value", "wrong").DisableDiff());
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
            async () => await VerifyInline("value").DisableDiff());
        Assert.Contains("InlineNew:", exception.Message);
        Assert.Contains("Received:", exception.Message);
        // The content block has no Expected section for a new snapshot
        Assert.DoesNotContain("\nExpected:\n", exception.Message);
    }

    [Fact]
    public async Task NonTextTarget()
    {
        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await VerifyInline("root", "root")
                .AppendContentAsFile(new byte[] { 1, 2, 3 }, "bin"));
        Assert.Contains("only support text", exception.Message);
    }

    [Fact]
    public void NonCsFile()
    {
        var settings = new VerifySettings();
        var exception = Assert.ThrowsAny<Exception>(
            () => settings.Inline("x", "Tests.vb", 1, "\"x\""));
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
        settings.Inline($"line1{eol}line2", FakeSource(), 1, "\"ignored\"");
        return Verify("line1\nline2", settings);
    }

    // Scrubbing normalizes received content, so a target containing CR still matches
    // an expected literal written with \n
    [Theory]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public Task ReceivedContentEolNormalized(string eol) =>
        VerifyInline(
            $"line1{eol}line2",
            """
            line1
            line2
            """);

    [Fact]
    public Task MultiTargetWithCrlfExpected()
    {
        var settings = new VerifySettings();
        settings.Inline(
            "---------- target#00.txt ----------\r\nroot\r\n---------- target#01.txt ----------\r\nextra",
            FakeSource(),
            1,
            "\"ignored\"");
        settings.AppendContentAsFile("extra");
        return Verify("root", settings);
    }

    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    public async Task AutoVerifyRewriteHonorsTemplateEol(string eol)
    {
        var template = WriteTemplate(mismatchTemplate, eol);
        try
        {
            var settings = new VerifySettings();
            settings.Inline("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("new1\nnew2", settings);

            var content = File.ReadAllText(template);
            var indent = new string(' ', 12);
            Assert.Contains(
                $"VerifyInline(value, \"\"\"{eol}{indent}new1{eol}{indent}new2{eol}{indent}\"\"\");",
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
        var template = WriteTemplate(
            """
            class Templ
            {
                void Method() =>
                    VerifyInline(value);
            }
            """,
            eol);
        try
        {
            var settings = new VerifySettings();
            settings.Inline(null, template, 4, null);
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("new1\nnew2", settings);

            var content = File.ReadAllText(template);
            var indent = new string(' ', 12);
            Assert.Contains(
                $"VerifyInline(value, \"\"\"{eol}{indent}new1{eol}{indent}new2{eol}{indent}\"\"\");",
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
            void Method() =>
                VerifyInline(value, "old");
        }
        """;

    [Fact]
    public async Task AutoVerifyMismatchRewritesSource()
    {
        var template = WriteTemplate(mismatchTemplate);
        try
        {
            var settings = new VerifySettings();
            settings.Inline("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            var content = File.ReadAllText(template);
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
        var template = WriteTemplate(
            """
            class Templ
            {
                void Method() =>
                    VerifyInline(value);
            }
            """);
        try
        {
            var settings = new VerifySettings();
            settings.Inline(null, template, 4, null);
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            Assert.Contains("VerifyInline(value, \"\"\"", File.ReadAllText(template));
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
                void Method() =>
                    VerifyInline(value, "newvalue");
            }
            """);
        try
        {
            var before = File.ReadAllText(template);
            var settings = new VerifySettings();
            settings.Inline("stale", template, 4, "\"stale\"");
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            Assert.Equal(before, File.ReadAllText(template));
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
                void Method() =>
                    VerifyInline(value, "different");
            }
            """);
        try
        {
            var settings = new VerifySettings();
            settings.Inline("stale", template, 4, "\"stale\"");
            settings.AutoVerify();
            settings.DisableDiff();
            var exception = await Assert.ThrowsAsync<VerifyException>(
                async () => await Verify("newvalue", settings));
            Assert.Contains("InlineNotEqual:", exception.Message);
            Assert.Contains("\"different\"", File.ReadAllText(template));
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
                void A() => VerifyInline(a, "oldA");
                void B() => VerifyInline(b, "oldB");
                void C() => VerifyInline(c, "oldC");
            }
            """);
        try
        {
            async Task Accept(string old, int line, string value)
            {
                var settings = new VerifySettings();
                settings.Inline(old, template, line, $"\"{old}\"");
                settings.AutoVerify();
                settings.DisableDiff();
                await Verify(value, settings);
            }

            await Task.WhenAll(
                Accept("oldA", 3, "newA"),
                Accept("oldB", 4, "newB"),
                Accept("oldC", 5, "newC"));

            var content = File.ReadAllText(template);
            Assert.Contains("newA", content);
            Assert.Contains("newB", content);
            Assert.Contains("newC", content);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task MovedToInlineCleanup()
    {
        var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "InlineScratch");
        Directory.CreateDirectory(directory);
        var stale = Path.Combine(directory, "MovedToInline.verified.txt");
        File.WriteAllText(stale, "stale");
        try
        {
            var settings = new VerifySettings();
            settings.UseDirectory("InlineScratch");
            settings.UseFileName("MovedToInline");
            settings.AutoVerify();
            settings.DisableDiff();
            settings.Inline("value", FakeSource(), 1, "\"value\"");
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

    [Fact]
    public async Task BuildServerDoesNotRewrite()
    {
        var template = WriteTemplate(mismatchTemplate);
        var original = File.ReadAllText(template);
        BuildServerDetector.Detected = true;
        try
        {
            var settings = new VerifySettings();
            settings.Inline("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();
            await Assert.ThrowsAsync<VerifyException>(
                async () => await Verify("newvalue", settings));
            Assert.Equal(original, File.ReadAllText(template));
        }
        finally
        {
            BuildServerDetector.Detected = false;
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }
}
