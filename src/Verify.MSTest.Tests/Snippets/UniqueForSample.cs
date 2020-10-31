using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using VerifyTests;

#region UniqueForSampleMSTest

[TestClass]
public class UniqueForSample :
    VerifyBase
{
    [TestMethod]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task RuntimeFluent()
    {
        return Verify("value")
            .UniqueForRuntime();
    }

    [TestMethod]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task RuntimeAndVersionFluent()
    {
        return Verify("value")
            .UniqueForRuntimeAndVersion();
    }

    [TestMethod]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task AssemblyConfigurationFluent()
    {
        return Verify("value")
            .UniqueForAssemblyConfiguration();
    }
}

#endregion