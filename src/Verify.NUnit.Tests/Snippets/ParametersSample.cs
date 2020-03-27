using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class ParametersSample
{
    #region NUnitTestCase
    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task TestCaseUsage(string arg)
    {
        return Verifier.Verify(arg);
    }
    #endregion
}