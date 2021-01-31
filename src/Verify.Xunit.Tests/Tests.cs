using System;
using System.Collections.Generic;
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

    [Theory]
    [MemberData(nameof(GetData))]
    public Task UseFileNameWithParam(string arg)
    {
        return Verifier.Verify(arg)
            .UseFileName("UseFileNameWithParam");
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
    }
}