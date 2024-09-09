#pragma warning disable InnerVerifyChecks
public class InnerVerifyChecksTests
{
    [Fact]
    public Task Valid() =>
        InnerVerifyChecks.Run(GetDirectory("Valid"));

    [Fact]
    public Task Invalid()
    {
        var directory = GetDirectory("Invalid");
        return ThrowsTask(() => InnerVerifyChecks.Run(directory));
    }

    static string GetDirectory(string suffix, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, suffix);
}