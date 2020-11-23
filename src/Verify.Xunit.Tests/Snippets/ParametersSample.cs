using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ParametersSample
{
    #region xunitInlineData

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsage(string arg)
    {
        VerifySettings settings = new();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsageFluent(string arg)
    {
        return Verifier.Verify(arg)
            .UseParameters(arg);
    }

    #endregion

    #region xunitMemberData

    [Theory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsage(string arg)
    {
        VerifySettings settings = new();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsageFluent(string arg)
    {
        return Verifier.Verify(arg)
            .UseParameters(arg);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
        yield return new object[] {"Value2"};
    }

    #endregion
}