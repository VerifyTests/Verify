#pragma warning disable InnerVerifyChecks
public class VerifyChecksTests
{
    static readonly string invalidDirectory;
    static readonly string partialDirectory;

    static List<string> extensions =
    [
        "txt",
        "json"
    ];

    static VerifyChecksTests()
    {
        invalidDirectory = GetDirectory("Invalid");
        partialDirectory = GetDirectory("Partial");
    }

    [Fact]
    public Task Superset()
    {
        var directory = GetDirectory("Superset");
        return InnerVerifyChecks.Run(directory);
    }

    [Fact]
    public Task Valid()
    {
        var directory = GetDirectory("Valid");
        return InnerVerifyChecks.Run(directory);
    }

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
        InnerVerifyChecks.CheckEditorConfig(invalidDirectory, extensions));

    [Fact]
    public Task PartialEditorConfig() =>
        ThrowsTask(() => InnerVerifyChecks.CheckEditorConfig(partialDirectory, extensions));

    [Fact]
    public Task PartialGitAttributes() =>
        ThrowsTask(() => InnerVerifyChecks.CheckGitAttributes(partialDirectory, extensions));

    static string GetDirectory(string suffix, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, suffix);
}