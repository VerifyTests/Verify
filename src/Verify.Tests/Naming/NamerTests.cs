using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class NamerTests
{
#if NET6_0 && DEBUG
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
            .ScrubLinesContaining("InnerVerifier.ValidatePrefix")
            .UseMethodName("ThrowOnConflict")
            .AddScrubber(builder => builder.Replace(@"\", "/"));
    }

    [Fact]
    public async Task DoesntThrowOnConflict()
    {
        static Task Run()
        {
            return Verifier.Verify("Value")
                .UseMethodName("Conflict")
                .DisableRequireUniquePrefix()
                .DisableDiff();
        }

        try
        {
            await Run();
        }
        catch
        {
        }

        await Verifier.Verify("Value")
            .UseMethodName("DoesntThrowOnConflict")
            .AddScrubber(builder => builder.Replace(@"\", "/"));
    }
#endif

    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify(Namer.Runtime, settings);
    }

    [Fact]
    public Task RuntimeFluent()
    {
        return Verifier.Verify(Namer.Runtime)
            .UniqueForRuntime();
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify(Namer.RuntimeAndVersion, settings);
    }

    [Fact]
    public Task RuntimeAndVersionFluent()
    {
        return Verifier.Verify(Namer.RuntimeAndVersion)
            .UniqueForRuntimeAndVersion();
    }

    [Fact]
    public Task TargetFramework()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFramework();
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFramework(typeof(ClassBeingTested).Assembly);
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkFluent()
    {
        return Verifier.Verify("Foo")
            .UniqueForTargetFramework();
    }

    [Fact]
    public Task TargetFrameworkFluentWithAssembly()
    {
        return Verifier.Verify("Foo")
            .UniqueForTargetFramework(typeof(ClassBeingTested).Assembly);
    }

    [Fact]
    public Task TargetFrameworkAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkAndVersionWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion(typeof(ClassBeingTested).Assembly);
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkAndVersionFluent()
    {
        return Verifier.Verify("Foo")
            .UniqueForTargetFrameworkAndVersion();
    }

    [Fact]
    public Task TargetFrameworkAndVersionFluentWithAssembly()
    {
        return Verifier.Verify("Foo")
            .UniqueForTargetFrameworkAndVersion(typeof(ClassBeingTested).Assembly);
    }

    [Fact]
    public async Task UseFileName()
    {
        #region UseFileName

        var settings = new VerifySettings();
        settings.UseFileName("CustomFileName");
        await Verifier.Verify("value", settings);

        #endregion
    }

    [Fact]
    public Task UseFileNameWithUnique()
    {
        var settings = new VerifySettings();
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

        var settings = new VerifySettings();
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

        var settings = new VerifySettings();
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

        var settings = new VerifySettings();
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
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("Foo", settings);
    }
    [Fact]
    public Task AssemblyConfigurationWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration(typeof(ClassBeingTested).Assembly);
        return Verifier.Verify("Foo", settings);
    }
    [Fact]
    public Task AssemblyConfigurationFluent()
    {
        return Verifier.Verify("Foo")
            .UniqueForAssemblyConfiguration();
    }

    [Fact]
    public Task AssemblyConfigurationFluentWithAssembly()
    {
        return Verifier.Verify("Foo")
            .UniqueForAssemblyConfiguration(typeof(ClassBeingTested).Assembly);
    }

    #region UseTextForParameters

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParameters(string arg)
    {
        var settings = new VerifySettings();
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
    public Task UseTextForParametersNoParam()
    {
        return Verifier.Verify("Value")
            .UseTextForParameters("Suffix");
    }

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
        var settings = new VerifySettings();
        settings.UniqueForArchitecture();
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task ArchitectureFluent()
    {
        return Verifier.Verify("Foo")
            .UniqueForArchitecture();
    }

    [Fact]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verifier.Verify("Foo", settings);
    }

    [Fact]
    public Task OSPlatformFluent()
    {
        return Verifier.Verify("Foo")
            .UniqueForOSPlatform();
    }
}