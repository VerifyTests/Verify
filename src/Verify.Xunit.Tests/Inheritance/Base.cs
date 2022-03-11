[UsesVerify]
public class Base
{
    [Fact]
    public Task TestInBase() =>
        Verify("Foo");

    [Fact]
    public virtual Task TestToOverride() =>
        Verify("Foo");
}