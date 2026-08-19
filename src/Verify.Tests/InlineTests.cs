// ReSharper disable ConstantExpected
[SuppressMessage("Performance", "CA1857:A constant is expected for the parameter")]
// Shared with InlineFSharpTests: both swap the global IsBuildServer, so running them in parallel
// has whichever finishes first restore the real detector under the other
[Collection("Inline")]
public class InlineTests :
    IDisposable
{
    // Every test below that accepts a snapshot rewrites source, which InlineEngine declines to do on
    // a build server. Pin the check off for the class, rather than have those tests pass locally and
    // fail on CI. The two build server tests move it back for their duration.
    Func<bool> originalIsBuildServer = InlineEngine.IsBuildServer;

    // Built on demand by WriteTemplate, so the tests that never write one neither create a
    // directory nor put a path in front of the temp path scrubber
    TempDirectory? templates;

    public InlineTests() =>
        InlineEngine.IsBuildServer = () => false;

    public void Dispose()
    {
        InlineEngine.IsBuildServer = originalIsBuildServer;
        templates?.Dispose();
    }

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

    #region InlineCombinationSample

    static string Concat(string a, int b) =>
        $"{a}{b}";

    [Fact]
    public Task Combinations() =>
        Combination()
            .Verify(
                Concat,
                ["a", "b"],
                [1, 2])
            .Snapshot(
                """
                {
                  a, 1: a1,
                  a, 2: a2,
                  b, 1: b1,
                  b, 2: b2
                }
                """);

    #endregion

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
    /// Nothing was produced, so the literal would be compared against nothing. That used to pass:
    /// Compare was never reached, and the verdict defaulted to Equal, so a snapshot that was never
    /// checked reported success and the result then had no text in it.
    /// </summary>
    [Fact]
    public async Task NoTargetsWithAnExplicitSnapshotSaysSo()
    {
        var settings = new VerifySettings();
        settings.Snapshot("expected", FakeSource(), 1, "\"expected\"");

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(new List<Target>(), settings));

        Assert.Contains("nothing to compare the snapshot against", exception.Message);
    }

    /// <summary>
    /// A registered string comparer decides equality here the same as it does for a verified
    /// file. An ordinal compare that stopped there meant a suite whose comparer passes against its
    /// files started failing the moment one of those snapshots moved inline, and nothing in the
    /// failure said the comparer had been skipped.
    /// </summary>
    [Fact]
    public Task StringComparerDecidesEquality() =>
        Verify("THE TEXT")
            .UseStringComparer(CaseInsensitive)
            .Snapshot("the text");

    /// <summary>
    /// And it is asked only when the two differ, so a comparer cannot make equal text unequal.
    /// </summary>
    [Fact]
    public async Task StringComparerIsNotAskedWhenTextMatches()
    {
        var asked = false;

        await Verify("value")
            .UseStringComparer(
                (_, _, _) =>
                {
                    asked = true;
                    return Task.FromResult(CompareResult.NotEqual("should not be asked"));
                })
            .Snapshot("value");

        Assert.False(asked);
    }

    static Task<CompareResult> CaseInsensitive(string received, string verified, IReadOnlyDictionary<string, object> context) =>
        Task.FromResult(new CompareResult(string.Equals(received, verified, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// A first target that differs tells the targets after it to stop trusting their comparers:
    /// they are usually derived from it, and a comparer that tolerates the difference would hide
    /// a real change in the source. The inlined target is compared outside the loop that feeds
    /// that cascade, so it was telling them nothing and the switch stopped working as soon as the
    /// source target was the inlined one.
    /// </summary>
    [Fact]
    public async Task ADifferingInlineFirstTargetBypassesComparersAfterIt()
    {
        using var directory = new TempDirectory();
        var asked = false;

        var settings = new VerifySettings();
        settings.DisableDiff();
        settings.UseDirectory(directory.Path);
        settings.UseFileName("Cascade");
        // Only the second target's extension, so the inlined one is not the thing being watched
        settings.UseStringComparer(
            (_, _, _) =>
            {
                asked = true;
                return Task.FromResult(CompareResult.Equal);
            },
            "json");
        settings.Snapshot("expected", FakeSource(), 1, "\"expected\"");

        // Content its verified file does not hold, so the comparer is what would decide it
        await File.WriteAllTextAsync(Path.Combine(directory.Path, "Cascade.verified.json"), "verified");

        List<Target> targets =
        [
            new("txt", new StringBuilder("received"))
            {
                BypassComparersForSubsequentOnDifference = true
            },
            new("json", new StringBuilder("second"))
        ];

        await Assert.ThrowsAsync<VerifyException>(() => Verify(targets, settings));

        Assert.False(asked);
    }

    /// <summary>
    /// Only the first target is inlined. The rest keep the names they would have had without
    /// inline, so the #01 file below is the same file it would be with no literal at all.
    /// </summary>
    [Fact]
    public async Task MultiTarget()
    {
        var result = await Verify("root")
            .AppendContentAsFile("extra")
            .Snapshot("root");

        // The snapshot is the first target, and the rest are files the same as they always were.
        // The result used to carry the snapshot alone, so anything reading Files to post-process
        // what a verification wrote found nothing there
        Assert.Equal("root", result.Text);
        Assert.Single(result.Files);
        Assert.EndsWith("MultiTarget#01.verified.txt", result.Files.Single());
    }

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

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task ParametersThrow(string value)
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await Verify(value)
                .Snapshot("a"));

        Assert.Contains("not compatible with parameterised tests", exception.Message);
        Assert.Contains($"_value={value}", exception.Message);
    }

    /// <summary>
    /// One literal serves every case, so the parameters have to be dropped from the verified
    /// name before the test can be inlined.
    /// </summary>
    #region InlineIgnoreParametersSample

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public Task IgnoredParameters(string value) =>
        Verify(value.Length)
            .IgnoreParameters()
            .Snapshot("1");

    #endregion

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public Task IgnoredParametersByName(string value) =>
        Verify(value.Length)
            .IgnoreParameters("value")
            .Snapshot("1");

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public Task IgnoredParametersForVerified(string value) =>
        Verify(value.Length)
            .IgnoreParametersForVerified()
            .Snapshot("1");

    /// <summary>
    /// Ignoring only some parameters still leaves the verified name varying per case.
    /// </summary>
    [Theory]
    [InlineData("a", 1)]
    [InlineData("b", 2)]
    public async Task PartiallyIgnoredParametersThrow(string value, int number)
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await Verify(value)
                .IgnoreParameters("value")
                .Snapshot("a"));

        Assert.Contains("not compatible with parameterised tests", exception.Message);
        Assert.Contains($"_number={number}", exception.Message);
        Assert.DoesNotContain("_value=", exception.Message);
    }

    /// <summary>
    /// Constructor arguments arrive as leading method parameters, with the class argument count
    /// separating them, so they gate inline in the same way.
    /// </summary>
    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task ConstructorParametersThrow(string classArg)
    {
        var settings = new VerifySettings();
        settings.SetClassArgumentCount(1);

        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await Verify(classArg, settings)
                .Snapshot("a"));

        Assert.Contains("not compatible with parameterised tests", exception.Message);
        Assert.Contains($"_classArg={classArg}", exception.Message);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public Task IgnoredConstructorParameters(string classArg)
    {
        var settings = new VerifySettings();
        settings.SetClassArgumentCount(1);
        return Verify(classArg.Length, settings)
            .IgnoreConstructorParameters()
            .Snapshot("1");
    }

    /// <summary>
    /// IgnoreConstructorParameters only drops the class arguments, so a method parameter
    /// alongside them still varies the verified name.
    /// </summary>
    [Theory]
    [InlineData("a", 1)]
    [InlineData("b", 2)]
    public async Task ConstructorAndMethodParametersThrow(string classArg, int number)
    {
        var settings = new VerifySettings();
        settings.SetClassArgumentCount(1);

        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await Verify(classArg, settings)
                .IgnoreConstructorParameters()
                .Snapshot("a"));

        Assert.Contains("not compatible with parameterised tests", exception.Message);
        Assert.Contains($"_number={number}", exception.Message);
        Assert.DoesNotContain("_classArg=", exception.Message);
    }

    [Fact]
    public async Task TextForParametersThrows()
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await Verify("value")
                .UseTextForParameters("case1")
                .Snapshot("value"));

        Assert.Contains("not compatible with parameterised tests", exception.Message);
        Assert.Contains("_case1", exception.Message);
    }

    /// <summary>
    /// UseFileName pins the verified name, so the parameters never reach it and every case
    /// already shares the one snapshot.
    /// </summary>
    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public Task FileNameDropsParameters(string value) =>
        Verify(value.Length)
            .UseFileName("InlineFileName")
            .Snapshot("1");

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task NotInlineBeatsParametersThrow(string value)
    {
        using var temp = new TempDirectory();
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.DisableDiff();
        settings.AutoVerify();
        settings.NotInline();
        settings.Snapshot("ignored", FakeSource(), 1, "\"ignored\"");

        var result = await Verify(value, settings);

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
    public void UnsupportedSourceLanguage()
    {
        var settings = new VerifySettings();
        var exception = Assert.ThrowsAny<Exception>(
            () => settings.Snapshot("x", "Tests.vb", 1, "\"x\""));
        Assert.Contains("C# and F# source files", exception.Message);
    }

    [Theory]
    [InlineData("Tests.cs")]
    [InlineData("Tests.fs")]
    [InlineData("Tests.fsx")]
    public void SupportedSourceLanguages(string file)
    {
        var settings = new VerifySettings();
        settings.Snapshot("x", file, 1, "\"x\"");
        Assert.NotNull(settings.inline);
    }

    // Deliberately not a real source file: a failing inline verify stages a patch, and
    // pointing it at real source would let a tray accept rewrite it
    static string FakeSource() =>
        Path.Combine(Path.GetTempPath(), "VerifyInlineFakeSource.cs");

    // xunit builds an instance per test, so the one template name cannot collide
    string WriteTemplate(string body)
    {
        templates ??= new();
        var path = templates.BuildPath("Template.cs");
        File.WriteAllText(path, body);
        return path;
    }

    // The template's line endings would otherwise be whatever the checkout produced
    string WriteTemplate(string body, string eol) =>
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
    // with any of these and must still match the \n normalized received text.
    // IgnoreParameters, since every case shares the one literal
    [Theory]
    [InlineData("\r\n")]
    [InlineData("\r")]
    [InlineData("\n")]
    public Task ExpectedLiteralEolNormalized(string eol)
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
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
            .IgnoreParameters()
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
            settings.IgnoreParameters();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("new1\nnew2", settings);

            var content = await File.ReadAllTextAsync(template);
            var indent = new string(' ', 12);
            Assert.Contains(
                $".Snapshot({eol}{indent}\"\"\"{eol}{indent}new1{eol}{indent}new2{eol}{indent}\"\"\");",
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
            settings.IgnoreParameters();
            settings.Snapshot(null, template, 4, null);
            settings.AutoVerify();
            settings.DisableDiff();

            await Verify("new1\nnew2", settings);

            var content = await File.ReadAllTextAsync(template);
            var indent = new string(' ', 12);
            Assert.Contains(
                $".Snapshot({eol}{indent}\"\"\"{eol}{indent}new1{eol}{indent}new2{eol}{indent}\"\"\");",
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

    const string defaultTemplate =
        """
        class Templ
        {
            Task Method() =>
                Verify(value).Snapshot(default);
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

    /// <summary>
    /// What the AutoVerify delegate is handed for an inline snapshot: the test source file, since
    /// that is what accepting rewrites, rather than anything shaped like a .verified path. A
    /// delegate deciding by that convention therefore declines every inline snapshot, which is
    /// worth pinning because it is the sort of thing a later change would break silently.
    /// </summary>
    [Fact]
    public async Task AutoVerifyIsHandedTheSourceFile()
    {
        var template = WriteTemplate(mismatchTemplate);
        try
        {
            var seen = new List<string>();
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.DisableDiff();
            settings.AutoVerify(
                file =>
                {
                    seen.Add(file);
                    return true;
                });

            await Verify("newvalue", settings);

            Assert.Equal(template, Assert.Single(seen));
            Assert.Contains("newvalue", await File.ReadAllTextAsync(template));
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
            // Single line content is a regular literal, not a raw one
            Assert.Contains(".Snapshot(\"newvalue\");", await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task AutoVerifyNewReplacesDefaultArgument()
    {
        // Passes either way - a lone call site is the one shape the content search did handle -
        // and is here to pin that routing `default` to the insertion path did not break it. That
        // path only understands the token from DiffEngine 20.0.0-beta.23 on; against an earlier
        // one this fails with "not a string literal", which is what the guard was waiting for
        var template = WriteTemplate(defaultTemplate);
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot(null, template, 4, "default");
            settings.AutoVerify();
            settings.DisableDiff();
            await Verify("newvalue", settings);
            Assert.Contains(".Snapshot(\"newvalue\");", await File.ReadAllTextAsync(template));
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

    /// <summary>
    /// The same stale-expression case as above, but where the expression is the bare `default`
    /// token. Content searching for it walks past the already-accepted call at the hint and lands
    /// on whichever other call site still says `default` - here another test in the same file,
    /// which then gets a snapshot that is not its own.
    /// </summary>
    [Fact]
    public async Task AutoVerifyDefaultDoesNotPatchAnotherTest()
    {
        var template = WriteTemplate(
            """
            class Templ
            {
                Task First() =>
                    Verify(value).Snapshot(default);

                Task Second() =>
                    Verify(value).Snapshot("newvalue");
            }
            """);
        try
        {
            var before = await File.ReadAllTextAsync(template);
            var settings = new VerifySettings();
            settings.Snapshot(null, template, 7, "default");
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
        // Scoped to the runtime: every target framework runs this concurrently against the same directory
        var name = $"MovedToInline_{Namer.RuntimeAndVersion}";
        var stale = Path.Combine(directory, $"{name}.verified.txt");
        await File.WriteAllTextAsync(stale, "stale");
        try
        {
            var settings = new VerifySettings();
            settings.UseDirectory("InlineScratch");
            settings.UseFileName(name);
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
            // Scoped to the runtime: every target framework runs this concurrently against the same directory
            var name = $"MovedToFile_{Namer.RuntimeAndVersion}";
            settings.UseFileName(name);

            await Verify("value", settings);

            Assert.DoesNotContain("Snapshot", await File.ReadAllTextAsync(template));
            var verified = Path.Combine(AttributeReader.GetProjectDirectory(), "InlineScratch", $"{name}.verified.txt");
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
        // Dispose puts the seam back
        InlineEngine.IsBuildServer = () => true;
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
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task BuildServerDoesNotRemoveTheLiteral()
    {
        var template = WriteTemplate(mismatchTemplate);
        var original = await File.ReadAllTextAsync(template);
        // Dispose puts the seam back
        InlineEngine.IsBuildServer = () => true;
        try
        {
            var settings = new VerifySettings();
            settings.Snapshot("old", template, 4, "\"old\"");
            settings.NotInline();
            settings.AutoVerify();
            settings.DisableDiff();
            settings.UseDirectory("InlineScratch");
            // Scoped to the runtime: every target framework runs this concurrently against the same directory
            var name = $"BuildServerNotInline_{Namer.RuntimeAndVersion}";
            settings.UseFileName(name);

            await Verify("value", settings);

            Assert.Equal(original, await File.ReadAllTextAsync(template));
            var verified = Path.Combine(AttributeReader.GetProjectDirectory(), "InlineScratch", $"{name}.verified.txt");
            if (File.Exists(verified))
            {
                File.Delete(verified);
            }
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }
}
