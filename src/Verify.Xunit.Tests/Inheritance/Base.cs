using VerifyXunit;
using Xunit;

[UsesVerify]
public class Base
{
    [Fact]
    public Task TestInBase()
    {
        return Verifier.Verify("Foo");
    }

    [Fact]
    public virtual Task TestToOverride()
    {
        return Verifier.Verify("Foo");
    }
}