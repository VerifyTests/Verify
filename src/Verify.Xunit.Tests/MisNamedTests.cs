using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

public class NoAttributeTests
{
    [Fact]
    public async Task ShouldThrow()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.Verify("Foo"));
        Assert.Contains("Unable to find type for file", exception.Message);
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