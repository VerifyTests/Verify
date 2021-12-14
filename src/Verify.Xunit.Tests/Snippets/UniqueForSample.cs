#region UniqueForSampleXunit

[UsesVerify]
public class UniqueForSample
{
    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    [Fact]
    public Task RuntimeFluent()
    {
        return Verify("value")
            .UniqueForRuntime();
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }

    [Fact]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }

    [Fact]
    public Task AssemblyConfigurationFluent()
    {
        return Verify("value")
            .UniqueForAssemblyConfiguration();
    }

    [Fact]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verify("value", settings);
    }

    [Fact]
    public Task ArchitectureFluent()
    {
        return Verify("value")
            .UniqueForArchitecture();
    }

    [Fact]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("value", settings);
    }

    [Fact]
    public Task OSPlatformFluent()
    {
        return Verify("value")
            .UniqueForOSPlatform();
    }
}

#endregion