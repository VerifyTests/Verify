using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Sdk;

public class NoAttributeTests
{
    [Fact]
    public async Task ShouldThrow()
    {
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("Foo"));
        Assert.Equal("Expected to find a `[InjectInfoAttribute]` on `MisNamedTests`.", exception.Message);
    }
}

[InjectInfo]
public class WithAttributeTests
{
    [Fact]
    public Task ShouldPass()
    {
        return Verifier.Verify("Foo");
    }
}