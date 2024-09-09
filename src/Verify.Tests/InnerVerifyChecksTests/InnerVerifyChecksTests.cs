#pragma warning disable InnerVerifyChecks
public class InnerVerifyChecksTests
{
    static readonly string invalidDirectory;

    static List<string> extensions =
    [
        "txt",
        "json"
    ];

    static InnerVerifyChecksTests() =>
        invalidDirectory = GetDirectory("Invalid");

    [Fact]
    public Task Valid() =>
        InnerVerifyChecks.Run(GetDirectory("Valid"));

    [Fact]
    public Task GetExtensions() =>
        Verify(InnerVerifyChecks.GetExtensions(AttributeReader.GetSolutionDirectory()));

    [Fact]
    public Task Invalid() =>
        ThrowsTask(() => InnerVerifyChecks.Run(invalidDirectory));

    [Fact]
    public Task CheckIncorrectlyImportedSnapshots() =>
        ThrowsTask(() => InnerVerifyChecks.CheckIncorrectlyImportedSnapshots(invalidDirectory));

    [Fact]
    public Task CheckGitIgnore() =>
        ThrowsTask(() => InnerVerifyChecks.CheckGitIgnore(invalidDirectory));

    [Fact]
    public Task CheckGitAttributes() =>
        ThrowsTask(() => InnerVerifyChecks.CheckGitAttributes(invalidDirectory, extensions));

    [Fact]
    public Task CheckEditorConfig() => ThrowsTask(() =>
        InnerVerifyChecks.CheckEditorConfig(invalidDirectory, extensions));

    static string GetDirectory(string suffix, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, suffix);
}