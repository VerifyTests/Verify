// Lives here, rather than in Verify.Tests, since the inline switch is a static setting, and this
// project runs serially with BaseTest resetting it between tests.
//
// The switch makes the verify calls below real inline call sites in this file. Nothing rewrites
// them: AutoVerify is off, so the only accept path is a review in DiffEngineViewer, and DiffRunner
// is disabled for the duration so no patch is even sent.
public class GlobalInlineTests :
    BaseTest,
    IDisposable
{
    bool diffDisabled = DiffEngine.DiffRunner.Disabled;
    Func<bool> isBuildServer = InlineEngine.IsBuildServer;

    public GlobalInlineTests()
    {
        DiffEngine.DiffRunner.Disabled = true;
        // The migration tests below rewrite source, which InlineEngine declines to do on a build
        // server. Pin the check off for the class, rather than have those tests pass locally and
        // fail on CI. MigrationSkippedOnBuildServer moves it back for its duration.
        InlineEngine.IsBuildServer = () => false;
    }

    // Built on demand by WriteTemplate, so the tests that never write one neither create a
    // directory nor put a path in front of the temp path scrubber
    TempDirectory? templates;

    public void Dispose()
    {
        DiffEngine.DiffRunner.Disabled = diffDisabled;
        InlineEngine.IsBuildServer = isBuildServer;
        templates?.Dispose();
    }

    [Fact]
    public async Task On()
    {
        VerifierSettings.Inline();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));

        Assert.Contains("InlineNew:", exception.Message);
        Assert.Contains("GlobalInlineTests.cs:", exception.Message);
    }

    /// <summary>
    /// A verification with no targets has no first target to inline, and indexing for one threw
    /// an ArgumentOutOfRangeException out of the switch. It declines, the way it declines
    /// everything else it cannot do, and an empty target list passes as it always has.
    /// </summary>
    [Fact]
    public async Task NoTargetsDeclinesRatherThanThrowing()
    {
        VerifierSettings.Inline();

        await Verify(new List<Target>());
    }

    [Fact]
    public async Task Off()
    {
        using var temp = new TempDirectory();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings(temp)));

        Assert.DoesNotContain("InlineNew:", exception.Message);
        Assert.Contains("New:", exception.Message);
    }

    /// <summary>
    /// The delegate is what lets a codebase turn this on for most tests and not others, so the
    /// context it gets has to be enough to make that call.
    /// </summary>
    [Fact]
    public async Task DelegateContext()
    {
        string? typeName = null;
        string? methodName = null;
        string? sourceFile = null;
        string? extension = null;
        VerifierSettings.Inline(
            (type, method, file, targetExtension) =>
            {
                typeName = type;
                methodName = method;
                sourceFile = file;
                extension = targetExtension;
                return true;
            });

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));

        Assert.Equal("GlobalInlineTests", typeName);
        Assert.Equal("DelegateContext", methodName);
        Assert.EndsWith("GlobalInlineTests.cs", sourceFile);
        Assert.Equal("txt", extension);
    }

    [Fact]
    public async Task DelegateDeclining()
    {
        VerifierSettings.Inline((_, _, _, _) => false);

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings(temp)));

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    /// <summary>
    /// Restricting the switch to plain text is the common reason to want it on for only part of a
    /// codebase, so the delegate is given the extension of the target that would be inlined.
    /// </summary>
    [Fact]
    public async Task DelegateFilteringByExtensionMatched()
    {
        VerifierSettings.Inline((_, _, _, extension) => extension == "txt");

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings(temp)));

        Assert.Contains("InlineNew:", exception.Message);
    }

    [Fact]
    public async Task DelegateFilteringByExtensionUnmatched()
    {
        VerifierSettings.Inline((_, _, _, extension) => extension == "txt");

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(new Target("xml", "<a />"), Settings(temp)));

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    [Fact]
    public async Task NotInlineBeatsTheSwitch()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var settings = Settings(temp);
        settings.NotInline();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    [Fact]
    public async Task NotInlineFluentBeatsTheSwitch()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            async () => await Verify("value")
                .UseDirectory(temp)
                .DisableDiff()
                .NotInline());

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    /// <summary>
    /// A converter that splits one input into several text targets has no sensible first target to
    /// inline, so it opts the whole verification out.
    /// </summary>
    [Fact]
    public async Task DontInlineFallsBackToFiles()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(
                new Target("txt", "page1")
                {
                    DontInline = true
                },
                Settings(temp)));

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    /// <summary>
    /// A binary target has no text to hold in a literal. The switch declines it, the way it
    /// declines everything else it cannot do: a codebase that turns inline on for everything
    /// still has tests that emit documents and images, and those must keep working untouched.
    /// An explicit Snapshot(...) still throws for them.
    /// </summary>
    [Fact]
    public async Task NonTextFirstTargetFallsBackToFiles()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(new MemoryStream([1, 2, 3]), "bin", Settings(temp)));

        Assert.DoesNotContain("only support text", exception.Message);
        Assert.DoesNotContain("InlineNew:", exception.Message);
        Assert.Contains("New:", exception.Message);
    }

    /// <summary>
    /// A combination captures its caller info at <c>Combination()</c>, so that is the line the hint
    /// names and the call an accept hangs the Snapshot off. The chained <c>Verify</c> is reached
    /// through a receiver of its own, which no entry point is, so it is not a candidate at all.
    /// </summary>
    [Fact]
    public async Task CombinationIsInlined()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Combination(settings: Settings(temp))
                .Verify(Concat, ["a", "b"], [1, 2]));

        Assert.Contains("InlineNew:", exception.Message);
    }

    static string Concat(string a, int b) =>
        $"{a}{b}";

    /// <summary>
    /// A test that reaches verify through a wrapper of its own. Accepting chains a Snapshot call
    /// onto the call written in the test, which only compiles where that call returns a
    /// SettingsTask, and a wrapper returning a Task does not. The switch cannot tell one from the
    /// other by looking at the verification, so it asks the source and declines.
    /// <para>
    /// sourceFile and lineNumber are ordinary optional parameters, which is how a wrapper points
    /// the snapshot at the test that called it, and how these tests stand one up.
    /// </para>
    /// </summary>
    [Fact]
    public async Task AWrapperCallSiteFallsBackToFiles()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify("value", Settings(temp), sourceFile: WrapperTemplate(), lineNumber: 3));

        Assert.DoesNotContain("InlineNew:", exception.Message);
        Assert.Contains("New:", exception.Message);
    }

    /// <summary>
    /// The same wrapper, once the test project has declared that it returns a SettingsTask.
    /// </summary>
    [Fact]
    public async Task ADeclaredWrapperIsInlined()
    {
        VerifierSettings.Inline();
        VerifierSettings.AddInlineEntryPoint("VerifyDocx");

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify("value", Settings(temp), sourceFile: WrapperTemplate(), lineNumber: 3));

        Assert.Contains("InlineNew:", exception.Message);
    }

    /// <summary>
    /// The control: the same shape of call site, with an entry point at it.
    /// </summary>
    [Fact]
    public async Task AnEntryPointCallSiteIsInlined()
    {
        VerifierSettings.Inline();

        var template = WriteTemplate(
            """
            class Templ
            {
                Task A() => Verify(value);
            }
            """);

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify("value", Settings(temp), sourceFile: template, lineNumber: 3));

        Assert.Contains("InlineNew:", exception.Message);
    }

    /// <summary>
    /// A source file that cannot be read says nothing about the call site, and taking that for a
    /// refusal would drop inline for a whole suite over an unrelated problem: a test assembly run
    /// from somewhere its sources were never deployed to still has a source path recorded in it.
    /// </summary>
    [Fact]
    public async Task AnUnreadableSourceFileIsStillInlined()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify("value", Settings(temp), sourceFile: temp.BuildPath("Gone.cs"), lineNumber: 3));

        Assert.Contains("InlineNew:", exception.Message);
    }

    [Fact]
    public void EntryPointsMustBeIdentifiers()
    {
        Assert.Throws<ArgumentException>(() => VerifierSettings.AddInlineEntryPoint("Verifier.VerifyDocx"));
        Assert.Throws<ArgumentException>(() => VerifierSettings.AddInlineEntryPoint("VerifyDocx(document)"));
        Assert.Throws<ArgumentException>(() => VerifierSettings.AddInlineEntryPoint(""));
    }

    string WrapperTemplate() =>
        WriteTemplate(
            """
            class Templ
            {
                Task A() => VerifyDocx(document);
            }
            """);

    /// <summary>
    /// One literal at one call site cannot hold a different value per test case, so the switch
    /// declines parameterised tests rather than breaking every data driven test in a codebase.
    /// An explicit Snapshot(...) still throws for them.
    /// </summary>
    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task ParametersAreNotInlined(string value)
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(value, Settings(temp)));

        Assert.DoesNotContain("InlineNew:", exception.Message);
        Assert.Contains("New:", exception.Message);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task ConstructorParametersAreNotInlined(string classArg)
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var settings = Settings(temp);
        settings.SetClassArgumentCount(1);

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(classArg, settings));

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    /// <summary>
    /// The ignore APIs collapse every case onto one verified snapshot, which is exactly what an
    /// inline literal can represent, so they bring the test back within the switch.
    /// </summary>
    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task IgnoredParametersAreInlined(string value)
    {
        VerifierSettings.Inline();

        var settings = new VerifySettings();
        settings.IgnoreParameters();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(value, settings));

        Assert.Contains("InlineNew:", exception.Message);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task GloballyIgnoredParametersAreInlined(string value)
    {
        VerifierSettings.Inline();
        VerifierSettings.IgnoreParameters();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(value));

        Assert.Contains("InlineNew:", exception.Message);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public async Task GloballyIgnoredConstructorParametersAreInlined(string classArg)
    {
        VerifierSettings.Inline();
        VerifierSettings.IgnoreConstructorParameters();

        var settings = new VerifySettings();
        settings.SetClassArgumentCount(1);

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(classArg, settings));

        Assert.Contains("InlineNew:", exception.Message);
    }

    /// <summary>
    /// UseUniqueDirectory has no single file to stand in for the snapshot. An explicit Snapshot(...)
    /// still throws for it, but the switch has to decline quietly rather than break the test.
    /// </summary>
    [Fact]
    public async Task UniqueDirectoryIsNotInlined()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var settings = Settings(temp);
        settings.UseUniqueDirectory();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    /// <summary>
    /// Only the first target is inlined; the others keep the file names they would have had, so
    /// flipping the switch never renames a snapshot file.
    /// </summary>
    [Fact]
    public async Task OnlyTheFirstTargetIsInlined()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var settings = Settings(temp);
        settings.AppendContentAsFile("extra");

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("root", settings));

        Assert.Contains("InlineNew:", exception.Message);
        // The inlined target leaves a gap where its #00 file would have been
        Assert.Contains("#01.verified.txt", exception.Message);
        Assert.DoesNotContain("#00.verified.txt", exception.Message);
    }

    /// <summary>
    /// The delegate cannot see the content, so a line limit is the only way to keep a long
    /// snapshot out of the test method it would otherwise swamp.
    /// </summary>
    [Fact]
    public async Task MaxLinesAtLimit()
    {
        VerifierSettings.Inline(maxLines: 3);

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(3)));

        Assert.Contains("InlineNew:", exception.Message);
    }

    [Fact]
    public async Task MaxLinesOverLimit()
    {
        VerifierSettings.Inline(maxLines: 3);

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(4), Settings(temp)));

        Assert.DoesNotContain("InlineNew:", exception.Message);
        Assert.Contains("New:", exception.Message);
    }

    /// <summary>
    /// Snapshots routinely end with a newline, and counting it would take a line off every
    /// budget for nothing.
    /// </summary>
    [Fact]
    public async Task MaxLinesIgnoresTrailingNewline()
    {
        VerifierSettings.Inline(maxLines: 2);

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("line1\nline2\n"));

        Assert.Contains("InlineNew:", exception.Message);
    }

    [Fact]
    public async Task MaxLinesNotSet()
    {
        VerifierSettings.Inline();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(500)));

        Assert.Contains("InlineNew:", exception.Message);
    }

    /// <summary>
    /// The two combine as an and: the delegate picks the candidate tests, and the limit then
    /// applies to what those produce. So the delegate still runs for content over the limit.
    /// </summary>
    [Fact]
    public async Task MaxLinesAppliesAfterTheDelegate()
    {
        var delegateRan = false;
        VerifierSettings.Inline(
            (_, _, _, _) =>
            {
                delegateRan = true;
                return true;
            },
            maxLines: 2);

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(3), Settings(temp)));

        Assert.True(delegateRan);
        Assert.DoesNotContain("InlineNew:", exception.Message);
    }

    /// <summary>
    /// A binary target has no lines to count, so the limit has nothing to say about it. It is
    /// declined for being binary, before the limit is ever reached.
    /// </summary>
    [Fact]
    public async Task MaxLinesWithBinaryFirstTarget()
    {
        VerifierSettings.Inline(maxLines: 1);

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(new MemoryStream([1, 2, 3]), "bin", Settings(temp)));

        Assert.DoesNotContain("only support text", exception.Message);
        Assert.DoesNotContain("InlineNew:", exception.Message);
        Assert.Contains("New:", exception.Message);
    }

    [Fact]
    public void MaxLinesInvalid() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => VerifierSettings.Inline(maxLines: 0));

    [Fact]
    public void ApplyMaxLinesToExistingRequiresMaxLines() =>
        Assert.Throws<ArgumentException>(() => VerifierSettings.Inline(applyMaxLinesToExisting: true));

    /// <summary>
    /// The limit routes new snapshots only. Removing a literal rewrites source, so an existing
    /// one is left alone until that is opted in to.
    /// </summary>
    [Fact]
    public async Task ExistingOverLimitKeptByDefault()
    {
        VerifierSettings.Inline(maxLines: 1);

        var template = WriteTemplate();
        try
        {
            using var temp = new TempDirectory();
            var settings = Settings(temp);
            settings.Snapshot("old", template, 3, "\"old\"");

            var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(2), settings));

            Assert.Contains("InlineNotEqual:", exception.Message);
            Assert.Contains("\"old\"", await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task ExistingUnderLimitStaysInline()
    {
        VerifierSettings.Inline(maxLines: 5, applyMaxLinesToExisting: true);

        var template = WriteTemplate();
        try
        {
            using var temp = new TempDirectory();
            var settings = Settings(temp);
            settings.Snapshot("old", template, 3, "\"old\"");

            var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(2), settings));

            Assert.Contains("InlineNotEqual:", exception.Message);
            Assert.Contains("\"old\"", await File.ReadAllTextAsync(template));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    /// <summary>
    /// The literal was the approved snapshot, so it seeds the verified file. Without that the
    /// migration reads as a brand new snapshot and the approved text is lost from both the
    /// source and the failure message.
    /// </summary>
    [Fact]
    public async Task ExistingOverLimitMovesToFile()
    {
        VerifierSettings.Inline(maxLines: 1, applyMaxLinesToExisting: true);

        var template = WriteTemplate();
        try
        {
            using var temp = new TempDirectory();
            var settings = Settings(temp);
            settings.Snapshot("old", template, 3, "\"old\"");

            var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(2), settings));

            Assert.DoesNotContain("Snapshot", await File.ReadAllTextAsync(template));
            Assert.DoesNotContain("InlineNotEqual:", exception.Message);
            Assert.Contains("NotEqual:", exception.Message);
            Assert.Equal("old", await File.ReadAllTextAsync(VerifiedPath(temp)));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    /// <summary>
    /// The limit covers every existing literal, not only ones whose content changed, so the
    /// migration has to leave a passing test passing.
    /// </summary>
    [Fact]
    public async Task UnchangedOverLimitMigratesWithoutFailing()
    {
        VerifierSettings.Inline(maxLines: 1, applyMaxLinesToExisting: true);

        var template = WriteTemplate();
        try
        {
            using var temp = new TempDirectory();
            var settings = Settings(temp);
            // Same content as Lines(2); Snapshot takes a constant
            settings.Snapshot("line1\nline2", template, 3, "\"old\"");

            await Verify(Lines(2), settings);

            Assert.DoesNotContain("Snapshot", await File.ReadAllTextAsync(template));
            Assert.Equal(Lines(2), await File.ReadAllTextAsync(VerifiedPath(temp)));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    [Fact]
    public async Task EmptySnapshotOverLimitMovesToFile()
    {
        VerifierSettings.Inline(maxLines: 1, applyMaxLinesToExisting: true);

        var template = WriteTemplate(
            """
            class Templ
            {
                Task A() => Verify(a).Snapshot();
            }
            """);
        try
        {
            using var temp = new TempDirectory();
            var settings = Settings(temp);
            settings.Snapshot(null, template, 3, null);

            var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(2), settings));

            Assert.DoesNotContain("Snapshot", await File.ReadAllTextAsync(template));
            Assert.DoesNotContain("InlineNew:", exception.Message);
            // Nothing to seed the verified file with, so it really is a new snapshot
            Assert.Contains("New:", exception.Message);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    /// <summary>
    /// Nothing is rewritten on a build server, so migrating there would leave the literal in the
    /// source and point the verification at a file that is not in the repository.
    /// </summary>
    [Fact]
    public async Task MigrationSkippedOnBuildServer()
    {
        InlineEngine.IsBuildServer = () => true;
        VerifierSettings.Inline(maxLines: 1, applyMaxLinesToExisting: true);

        var template = WriteTemplate();
        var original = await File.ReadAllTextAsync(template);
        try
        {
            using var temp = new TempDirectory();
            var settings = Settings(temp);
            settings.Snapshot("old", template, 3, "\"old\"");

            var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(2), settings));

            Assert.Contains("InlineNotEqual:", exception.Message);
            Assert.Equal(original, await File.ReadAllTextAsync(template));
            Assert.False(File.Exists(VerifiedPath(temp)));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    /// <summary>
    /// Several inline verifies per method are legal, since a literal has no file name to collide
    /// on. Migrating puts them back under the file naming rules, where they do.
    /// </summary>
    [Fact]
    public async Task TwoMigratingSnapshotsInOneMethodThrow()
    {
        VerifierSettings.Inline(maxLines: 1, applyMaxLinesToExisting: true);

        var template = WriteTemplate(
            """
            class Templ
            {
                Task A() => Verify(a).Snapshot("old");
                Task B() => Verify(b).Snapshot("old");
            }
            """);
        try
        {
            using var temp = new TempDirectory();

            VerifySettings SiteSettings(int line)
            {
                var settings = Settings(temp);
                settings.Snapshot("old", template, line, "\"old\"");
                return settings;
            }

            await Assert.ThrowsAsync<VerifyException>(() => Verify(Lines(2), SiteSettings(3)));
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => Verify(Lines(2), SiteSettings(4)));

            Assert.Contains("The prefix has already been used", exception.Message);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(template)!, true);
        }
    }

    // Content with a known line count, for exercising maxLines
    static string Lines(int count) =>
        string.Join('\n', Enumerable.Range(1, count).Select(_ => $"line{_}"));

    static string VerifiedPath(TempDirectory temp, [CallerMemberName] string name = "") =>
        temp.BuildPath($"{nameof(GlobalInlineTests)}.{name}.verified.txt");

    // Migrating away from inline rewrites source, so the tests that exercise it point at a
    // throwaway file rather than this one. xunit builds an instance per test, so the one
    // template name cannot collide
    string WriteTemplate(string body = snapshotTemplate)
    {
        templates ??= new();
        var path = templates.BuildPath("Template.cs");
        File.WriteAllText(path, body);
        return path;
    }

    const string snapshotTemplate =
        """
        class Templ
        {
            Task A() => Verify(a).Snapshot("old");
        }
        """;

    static VerifySettings Settings(TempDirectory temp)
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        // Every verify here is expected to fail, so without this the diff tool is launched
        settings.DisableDiff();
        return settings;
    }
}
