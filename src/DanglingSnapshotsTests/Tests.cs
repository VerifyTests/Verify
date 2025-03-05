#pragma warning disable InnerVerifyChecks
public class Tests
{
    [ModuleInitializer]
    public static void Init() =>
        Environment.SetEnvironmentVariable("CI", "true");

    [Fact]
    public Task Simple() =>
        Verify("Foo");

    [Fact]
    public Task RunChecks()
    {
        var directory = GetDirectory("Valid");
        return InnerVerifyChecks.Run(directory, directory);
    }

    static string GetDirectory(string suffix, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, suffix);
}