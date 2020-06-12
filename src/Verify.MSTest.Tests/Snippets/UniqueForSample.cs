using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;
using VerifyMSTest;

#region UniqueForSampleMSTest
[TestClass]
public class UniqueForSample
{
    [TestMethod]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [TestMethod]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify("value", settings);
    }

    [TestMethod]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("value", settings);
    }
}
#endregion