
#pragma warning disable InnerVerifyChecks
[TestFixture]
public class Tests
{
    [ModuleInitializer]
    public static void Init() =>
        Environment.SetEnvironmentVariable("CI", "true");

    [Test]
    public Task Simple() =>
        Verify("Foo");

    [Test]
    public Task RunChecks()
    {
        var directory = GetDirectory("Valid");
        return InnerVerifyChecks.Run(directory, directory);
    }

    static string GetDirectory(string suffix, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, suffix);
}