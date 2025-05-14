#pragma warning disable InnerVerifyChecks
public class VerifyChecksTests
{
    static readonly string invalidDirectory;
    static readonly string partialDirectory;

    static List<string> extensions =
    [
        "txt",
        "json",
        "bin"
    ];

    static List<string> editorConfigExtensions = extensions.Except("bin").ToList();

    static VerifyChecksTests()
    {
        invalidDirectory = GetDirectory("Invalid");
        partialDirectory = GetDirectory("Partial");
    }

    [Fact]
    public Task Superset() =>
        InnerVerifyChecks.Run(GetDirectory("Superset"));

    [Fact]
    public Task Valid() =>
        InnerVerifyChecks.Run(GetDirectory("Valid"));

    [Fact]
    public Task Invalid() =>
        ThrowsTask(() => InnerVerifyChecks.Run(invalidDirectory));

    [Fact]
    public Task GetExtensions() =>
        Verify(InnerVerifyChecks.GetExtensions(AttributeReader.GetSolutionDirectory()));

    [Fact]
    public Task IncorrectlyImportedSnapshots() =>
        ThrowsTask(() => InnerVerifyChecks.CheckIncorrectlyImportedSnapshots(invalidDirectory));

    [Fact]
    public Task GitIgnore() =>
        ThrowsTask(() => InnerVerifyChecks.CheckGitIgnore(invalidDirectory));

    [Fact]
    public Task GitAttributes() =>
        ThrowsTask(() => InnerVerifyChecks.CheckGitAttributes(invalidDirectory, extensions));

    [Fact]
    public Task EditorConfig() => ThrowsTask(() =>
        InnerVerifyChecks.CheckEditorConfig(invalidDirectory, editorConfigExtensions));

    [Fact]
    public Task PartialEditorConfig() =>
        ThrowsTask(() => InnerVerifyChecks.CheckEditorConfig(partialDirectory, editorConfigExtensions));

    [Fact]
    public Task PartialGitAttributes() =>
        ThrowsTask(() => InnerVerifyChecks.CheckGitAttributes(partialDirectory, extensions));

    static string GetDirectory(string suffix, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, suffix);
}