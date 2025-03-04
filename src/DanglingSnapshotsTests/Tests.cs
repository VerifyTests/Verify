public class Tests
{
    [Fact]
    public Task Simple() =>
        Verify("Foo");

    [Fact]
    public Task Checks() =>
        VerifyChecks.Run();
}