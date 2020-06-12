using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;
using VerifyMSTest;

[TestClass]
public class ParametersSample
{
    #region MSTestDataRow
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
    }
    #endregion
}