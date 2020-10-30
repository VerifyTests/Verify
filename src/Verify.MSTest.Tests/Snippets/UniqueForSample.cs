using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

#region UniqueForSampleMSTest
[TestClass]
public class UniqueForSample :
    VerifyBase
{
    [TestMethod]
    public Task Runtime()
    {
        return Verify("value")
            .UniqueForRuntime();
    }

    [TestMethod]
    public Task RuntimeAndVersion()
    {
        return Verify("value")
            .UniqueForRuntimeAndVersion();
    }

    [TestMethod]
    public Task AssemblyConfiguration()
    {
        return Verify("value")
            .UniqueForAssemblyConfiguration();
    }
}
#endregion