using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ParametersSample :
    VerifyBase
{
    #region xunitInlineData
    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsage(string arg)
    {
        return Verify(arg);
    }
    #endregion

    #region xunitMemberData
    [Theory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsage(string arg)
    {
        return Verify(arg);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
        yield return new object[] {"Value2"};
    }
    #endregion

    public ParametersSample(ITestOutputHelper output) :
        base(output)
    {
    }
}