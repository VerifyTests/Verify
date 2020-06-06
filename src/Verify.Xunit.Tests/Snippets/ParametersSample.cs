using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
public class ParametersSample
{
    #region xunitInlineData
    [VerifyTheory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsage(string arg)
    {
        return Verifier.Verify(arg);
    }
    #endregion

    #region xunitMemberData
    [VerifyTheory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsage(string arg)
    {
        return Verifier.Verify(arg);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
        yield return new object[] {"Value2"};
    }
    #endregion
}