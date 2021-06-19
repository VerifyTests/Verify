﻿using System.Diagnostics;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class NamerTests
{
#if NET6_0
    [Fact]
    public async Task ThrowOnConflict()
    {
        static Task Run()
        {
            return Verifier.Verify("Value")
                .UseMethodName("Conflict")
                .DisableDiff();
        }

        try
        {
            await Run();
        }
        catch
        {
        }

        await Verifier.ThrowsTask(Run)
            .UseMethodName("ThrowOnConflict")
            .AddScrubber(builder => builder.Replace(@"\", "/"));
    }
#endif

    [Fact]
    public Task Runtime()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntime();
        return Verifier.Verify(Namer.Runtime, settings);
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify(Namer.RuntimeAndVersion, settings);
    }

    [Fact]
    public async Task UseFileName()
    {
        #region UseFileName

        VerifySettings settings = new();
        settings.UseFileName("CustomFileName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public Task UseFileNameWithUnique()
    {
        VerifySettings settings = new();
        settings.UseFileName("CustomFileName");
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public async Task UseFileNameFluent()
    {
        #region UseFileNameFluent

        await Verifier.Verify("value")
            .UseFileName("CustomFileNameFluent");

        #endregion
    }

    [Fact]
    public async Task UseDirectory()
    {
        #region UseDirectory

        VerifySettings settings = new();
        settings.UseDirectory("CustomDirectory");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseDirectoryFluent()
    {
        #region UseDirectoryFluent

        await Verifier.Verify("value")
            .UseDirectory("CustomDirectory");

        #endregion
    }

    [Fact]
    public async Task UseTypeName()
    {
        #region UseTypeName

        VerifySettings settings = new();
        settings.UseTypeName("CustomTypeName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseTypeNameFluent()
    {
        #region UseTypeNameFluent

        await Verifier.Verify("value")
            .UseTypeName("CustomTypeName");

        #endregion
    }

    [Fact]
    public async Task UseMethodName()
    {
        #region UseMethodName

        VerifySettings settings = new();
        settings.UseMethodName("CustomMethodName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseMethodNameFluent()
    {
        #region UseMethodNameFluent

        await Verifier.Verify("value")
            .UseMethodName("CustomMethodNameFluent");

        #endregion
    }

    [Fact]
    public void AccessNamerRuntimeAndVersion()
    {
        #region AccessNamerRuntimeAndVersion

        Debug.WriteLine(Namer.Runtime);
        Debug.WriteLine(Namer.RuntimeAndVersion);

        #endregion
    }

    [Fact]
    public Task AssemblyConfiguration()
    {
        VerifySettings settings = new();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("Foo", settings);
    }

    #region UseTextForParameters

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParameters(string arg)
    {
        VerifySettings settings = new();
        settings.UseTextForParameters(arg);
        return Verifier.Verify(arg, settings);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParametersFluent(string arg)
    {
        return Verifier.Verify(arg)
            .UseTextForParameters(arg);
    }

    #endregion

    [Fact]
    public void AccessNamerArchitecture()
    {
        #region AccessNamerArchitecture

        Debug.WriteLine(Namer.Architecture);

        #endregion
    }

    [Fact]
    public Task Architecture()
    {
        VerifySettings settings = new();
        settings.UniqueForArchitecture();
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task OSPlatform()
    {
        VerifySettings settings = new();
        settings.UniqueForOSPlatform();
        return Verifier.Verify("Foo", settings);
    }
}