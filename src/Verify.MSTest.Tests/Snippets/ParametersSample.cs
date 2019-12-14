using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;
using VerifyMSTest;

[TestClass]
public class ParametersSample :
    VerifyBase
{
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task Usage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }
}