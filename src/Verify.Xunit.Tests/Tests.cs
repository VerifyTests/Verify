using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    [Theory]
    [InlineData("Value1")]
    public async Task MissingParameter(string arg)
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.Verify("Foo"));
        Assert.Contains("requires parameters", exception.Message);
    }
}