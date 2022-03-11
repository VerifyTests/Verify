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
    public Task RuntimeFluent() =>
        Verify("value")
            .UniqueForRuntime();

    [TestMethod]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task RuntimeAndVersionFluent() =>
        Verify("value")
            .UniqueForRuntimeAndVersion();

    [TestMethod]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task AssemblyConfigurationFluent() =>
        Verify("value")
            .UniqueForAssemblyConfiguration();

    [TestMethod]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task ArchitectureFluent() =>
        Verify("value")
            .UniqueForArchitecture();

    [TestMethod]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("value", settings);
    }

    [TestMethod]
    public Task OSPlatformFluent() =>
        Verify("value")
            .UniqueForOSPlatform();
}

#endregion