using System.Collections.Generic;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
public class ParametersSample
{
    #region xunitInlineData
    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
    }
    #endregion

    #region xunitMemberData
    [Theory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
        yield return new object[] {"Value2"};
    }
    #endregion
}