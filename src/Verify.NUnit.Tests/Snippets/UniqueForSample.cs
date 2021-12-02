#region UniqueForSampleNUnit

[TestFixture]
public class UniqueForSample
{
    [Test]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task RuntimeFluent()
    {
        return Verifier.Verify("value")
            .UniqueForRuntime();
    }

    [Test]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task AssemblyConfigurationFluent()
    {
        return Verifier.Verify("value")
            .UniqueForAssemblyConfiguration();
    }

    [Test]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task RuntimeAndVersionFluent()
    {
        return Verifier.Verify("value")
            .UniqueForRuntimeAndVersion();
    }

    [Test]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task ArchitectureFluent()
    {
        return Verifier.Verify("value")
            .UniqueForArchitecture();
    }

    [Test]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task OSPlatformFluent()
    {
        return Verifier.Verify("value")
            .UniqueForOSPlatform();
    }
}

#endregion