#region UniqueForSampleFixie

public class UniqueForSample
{
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    public Task RuntimeFluent() =>
        Verify("value")
            .UniqueForRuntime();

    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }

    public Task AssemblyConfigurationFluent() =>
        Verify("value")
            .UniqueForAssemblyConfiguration();

    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }

    public Task RuntimeAndVersionFluent() =>
        Verify("value")
            .UniqueForRuntimeAndVersion();

    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verify("value", settings);
    }

    public Task ArchitectureFluent() =>
        Verify("value")
            .UniqueForArchitecture();

    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("value", settings);
    }

    public Task OSPlatformFluent() =>
        Verify("value")
            .UniqueForOSPlatform();
}

#endregion