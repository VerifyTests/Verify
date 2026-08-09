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

    public GlobalInlineTests() =>
        DiffEngine.DiffRunner.Disabled = true;

    public void Dispose() =>
        DiffEngine.DiffRunner.Disabled = diffDisabled;

    [Fact]
    public async Task On()
    {
        VerifierSettings.Inline();

        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));

        Assert.Contains("InlineNew:", exception.Message);
        Assert.Contains("GlobalInlineTests.cs:", exception.Message);
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

    [Fact]
    public async Task NonTextFirstTargetThrows()
    {
        VerifierSettings.Inline();

        using var temp = new TempDirectory();
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(new MemoryStream([1, 2, 3]), "bin", Settings(temp)));

        Assert.Contains("only support text", exception.Message);
        Assert.Contains("NotInline", exception.Message);
        Assert.Contains("DontInline", exception.Message);
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

    static VerifySettings Settings(TempDirectory temp)
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        // Every verify here is expected to fail, so without this the diff tool is launched
        settings.DisableDiff();
        return settings;
    }
}
