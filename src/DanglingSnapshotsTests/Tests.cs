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
}