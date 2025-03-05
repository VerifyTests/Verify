public class Tests
{
    [ModuleInitializer]
    public static void Init() =>
        Environment.SetEnvironmentVariable("CI", "true");

    [Fact]
    public Task Simple() =>
        Verify("Foo");

    [Fact]
    public Task Checks() =>
        VerifyChecks.Run();
}