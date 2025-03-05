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
    public Task RunChecks() =>
        InnerVerifyChecks.Run(GetDirectory());

    static string GetDirectory([CallerFilePath] string sourceFile = "") =>
        Path.GetDirectoryName(sourceFile)!;
}