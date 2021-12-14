[UsesVerify]
public class Base
{
    [Fact]
    public Task TestInBase()
    {
        return Verify("Foo");
    }

    [Fact]
    public virtual Task TestToOverride()
    {
        return Verify("Foo");
    }
}