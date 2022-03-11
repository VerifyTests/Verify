[UsesVerify]
public class Tests
{
    [Fact]
    public Task Simple() =>
        Verify("Foo");
}