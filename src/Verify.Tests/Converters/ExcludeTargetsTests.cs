public class ExcludeTargetsTests
{
    // Mimics a document converter (eg Verify.PdfPig) that emits the source document
    // alongside targets derived from it.
    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.RegisterStreamConverter(
            "excludesource",
            (_, _, _) =>
                new(
                    null,
                    [
                        new("excludesource", new MemoryStream("source-document"u8.ToArray())),
                        new("txt", "derived from source")
                    ]));

    class SourceDocument;

    // The same shape, reached through a file converter rather than a stream converter.
    [ModuleInitializer]
    public static void FileConverterInit() =>
        VerifierSettings.RegisterFileConverter<SourceDocument>(
            (_, _) =>
                new(
                    "the info",
                    [new("excludedbin", new MemoryStream("source-document"u8.ToArray()))]));

    static Target[] BuildTargets() =>
    [
        new("excludedbin", new MemoryStream("binary-content"u8.ToArray()))
    ];

    // Only the root txt target survives, so no verified file is required for the excluded extension.
    [Fact]
    public async Task ExcludeStreamTarget()
    {
        var result = await Verify(null, BuildTargets())
            .ExcludeTargets("excludedbin")
            .DisableDiff();
        Assert.Single(result.Files);
    }

    [Fact]
    public async Task ExtensionMatchIsCaseInsensitive()
    {
        var result = await Verify(null, BuildTargets())
            .ExcludeTargets("EXCLUDEDBIN")
            .DisableDiff();
        Assert.Single(result.Files);
    }

    // The engine disposes the streams it consumes. An excluded stream never reaches it,
    // so the exclusion has to dispose it instead.
    [Fact]
    public async Task ExcludedStreamIsDisposed()
    {
        var stream = new MemoryStream("binary-content"u8.ToArray());
        Target[] targets = [new("excludedbin", stream)];
        await Verify(null, targets)
            .ExcludeTargets("excludedbin")
            .DisableDiff();
        Assert.False(stream.CanRead);
    }

    // The source document a converter emits can be excluded, keeping its derived targets.
    // Were the exclusion to fail, an unexpected excludesource target would be reported as new.
    #region ExcludeTargets

    [Fact]
    public Task ExcludeConverterSourceTarget() =>
        Verify(new MemoryStream("source-document"u8.ToArray()), "excludesource")
            .ExcludeTargets("excludesource");

    #endregion

    [Fact]
    public async Task ExcludeFileConverterSourceTarget()
    {
        var result = await Verify(new SourceDocument())
            .ExcludeTargets("excludedbin")
            .DisableDiff();
        Assert.Single(result.Files);
    }

    [Fact]
    public async Task ExcludingAllTargetsThrows()
    {
        var exception = await Assert.ThrowsAsync<Exception>(
            () => Verify(new MemoryStream("binary-content"u8.ToArray()), "excludedbin")
                .ExcludeTargets("excludedbin")
                .DisableDiff());
        Assert.Contains("All targets have been excluded", exception.Message);
    }

    [Fact]
    public void NoExtensionsThrows() =>
        Assert.Throws<ArgumentException>(() => new VerifySettings().ExcludeTargets());

    [Fact]
    public void ExtensionWithPeriodThrows() =>
        Assert.Throws<ArgumentException>(() => new VerifySettings().ExcludeTargets(".pdf"));
}
