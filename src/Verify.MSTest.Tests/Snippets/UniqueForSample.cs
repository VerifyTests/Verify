﻿using System.Threading.Tasks;
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
        VerifySettings settings = new();
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
        VerifySettings settings = new();
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
        VerifySettings settings = new();
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
        VerifySettings settings = new();
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
        VerifySettings settings = new();
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