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
    public Task RuntimeFluent() =>
        Verify("value")
            .UniqueForRuntime();

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
    public Task AssemblyConfigurationFluent() =>
        Verify("value")
            .UniqueForAssemblyConfiguration();

    [Fact]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verify("value", settings);
    }

    [Fact]
    public Task ArchitectureFluent() =>
        Verify("value")
            .UniqueForArchitecture();

    [Fact]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("value", settings);
    }

    [Fact]
    public Task OSPlatformFluent() =>
        Verify("value")
            .UniqueForOSPlatform();
}

#endregion