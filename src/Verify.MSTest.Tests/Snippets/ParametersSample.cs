using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;
using VerifyMSTest;

[TestClass]
public class ParametersSample :
    VerifyBase
{
    #region MSTestDataRow
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsage(string arg)
    {
        return Verify(arg)
            .UseParameters(arg);
    }
    #endregion
}