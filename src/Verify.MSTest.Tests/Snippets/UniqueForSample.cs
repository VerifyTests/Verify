namespace TheTests;

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

    [TestMethod]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task ArchitectureFluent()
    {
        return Verify("value")
            .UniqueForArchitecture();
    }

    [TestMethod]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task OSPlatformFluent()
    {
        return Verify("value")
            .UniqueForOSPlatform();
    }
}

#endregion