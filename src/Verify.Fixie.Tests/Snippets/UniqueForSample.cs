﻿#region UniqueForSampleNUnit

[TestFixture]
public class UniqueForSample
{
    [Test]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    [Test]
    public Task RuntimeFluent() =>
        Verify("value")
            .UniqueForRuntime();

    [Test]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }

    [Test]
    public Task AssemblyConfigurationFluent() =>
        Verify("value")
            .UniqueForAssemblyConfiguration();

    [Test]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }

    [Test]
    public Task RuntimeAndVersionFluent() =>
        Verify("value")
            .UniqueForRuntimeAndVersion();

    [Test]
    public Task Architecture()
    {
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verify("value", settings);
    }

    [Test]
    public Task ArchitectureFluent() =>
        Verify("value")
            .UniqueForArchitecture();

    [Test]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("value", settings);
    }

    [Test]
    public Task OSPlatformFluent() =>
        Verify("value")
            .UniqueForOSPlatform();
}

#endregion