#region UniqueForSampleXunit

[UsesVerify]
public class UniqueForSample
{
    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task RuntimeFluent()
    {
        return Verifier.Verify("value")
            .UniqueForRuntime();
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task AssemblyConfigurationFluent()
    {
        return Verifier.Verify("value")
            .UniqueForAssemblyConfiguration();
    }

    [Fact]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task ArchitectureFluent()
    {
        return Verifier.Verify("value")
            .UniqueForArchitecture();
    }

    [Fact]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task OSPlatformFluent()
    {
        return Verifier.Verify("value")
            .UniqueForOSPlatform();
    }
}

#endregion