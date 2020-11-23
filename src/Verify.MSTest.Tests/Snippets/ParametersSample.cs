using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;
using VerifyMSTest;

#region MSTestDataRow

[TestClass]
public class ParametersSample :
    VerifyBase
{
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsage(string arg)
    {
        VerifySettings settings = new();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsageFluent(string arg)
    {
        return Verify(arg)
            .UseParameters(arg);
    }
}

#endregion