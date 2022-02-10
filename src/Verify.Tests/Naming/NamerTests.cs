[UsesVerify]
public class NamerTests
{
#if NET6_0 && DEBUG
    [Fact]
    public async Task ThrowOnConflict()
    {
        static Task Run()
        {
            return Verify("Value")
                .UseMethodName("Conflict1")
                .DisableDiff();
        }

        try
        {
            await Run();
        }
        catch
        {
        }

        await ThrowsTask(Run)
            .ScrubLinesContaining("InnerVerifier.ValidatePrefix")
            .UseMethodName("ThrowOnConflict")
            .AddScrubber(builder => builder.Replace(@"\", "/"));
    }

    [Fact]
    public async Task DoesntThrowOnConflict()
    {
        static Task Run()
        {
            return Verify("Value")
                .UseMethodName("Conflict2")
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

        await Verify("Value")
            .UseMethodName("DoesntThrowOnConflict")
            .AddScrubber(builder => builder.Replace(@"\", "/"));
    }
#endif

#region MultipleCalls

    [Fact]
    public async Task MultipleCalls()
    {
        await Task.WhenAll(
            Verify("Value1")
                .UseMethodName("MultipleCalls_1"),
            Verify("Value1")
                .UseMethodName("MultipleCalls_2"));
    }

#endregion

    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify(Namer.Runtime, settings);
    }

    [Fact]
    public Task RuntimeFluent()
    {
        return Verify(Namer.Runtime)
            .UniqueForRuntime();
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify(Namer.RuntimeAndVersion, settings);
    }

    [Fact]
    public Task RuntimeAndVersionFluent()
    {
        return Verify(Namer.RuntimeAndVersion)
            .UniqueForRuntimeAndVersion();
    }

    [Fact]
    public Task TargetFramework()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFramework();
        return Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFramework(typeof(ClassBeingTested).Assembly);
        return Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkFluent()
    {
        return Verify("Foo")
            .UniqueForTargetFramework();
    }

    [Fact]
    public Task TargetFrameworkFluentWithAssembly()
    {
        return Verify("Foo")
            .UniqueForTargetFramework(typeof(ClassBeingTested).Assembly);
    }

    [Fact]
    public Task TargetFrameworkAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        return Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkAndVersionWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion(typeof(ClassBeingTested).Assembly);
        return Verify("Foo", settings);
    }

    [Fact]
    public Task TargetFrameworkAndVersionFluent()
    {
        return Verify("Foo")
            .UniqueForTargetFrameworkAndVersion();
    }

    [Fact]
    public Task TargetFrameworkAndVersionFluentWithAssembly()
    {
        return Verify("Foo")
            .UniqueForTargetFrameworkAndVersion(typeof(ClassBeingTested).Assembly);
    }

    [Fact]
    public async Task UseFileName()
    {
        #region UseFileName

        var settings = new VerifySettings();
        settings.UseFileName("CustomFileName");
        await Verify("value", settings);

        #endregion
    }

    [Fact]
    public Task UseFileNameWithUnique()
    {
        var settings = new VerifySettings();
        settings.UseFileName("CustomFileName");
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    [Fact]
    public async Task UseFileNameFluent()
    {
        #region UseFileNameFluent

        await Verify("value")
            .UseFileName("CustomFileNameFluent");

        #endregion
    }

    [Fact]
    public async Task UseDirectory()
    {
        #region UseDirectory

        var settings = new VerifySettings();
        settings.UseDirectory("CustomDirectory");
        await Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseDirectoryFluent()
    {
        #region UseDirectoryFluent

        await Verify("value")
            .UseDirectory("CustomDirectory");

        #endregion
    }

    [Fact]
    public async Task UseTypeName()
    {
        #region UseTypeName

        var settings = new VerifySettings();
        settings.UseTypeName("CustomTypeName");
        await Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseTypeNameFluent()
    {
        #region UseTypeNameFluent

        await Verify("value")
            .UseTypeName("CustomTypeName");

        #endregion
    }

    [Fact]
    public async Task UseMethodName()
    {
        #region UseMethodName

        var settings = new VerifySettings();
        settings.UseMethodName("CustomMethodName");
        await Verify("value", settings);

        #endregion
    }

    [Fact]
    public async Task UseMethodNameFluent()
    {
        #region UseMethodNameFluent

        await Verify("value")
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
        return Verify("Foo", settings);
    }
    [Fact]
    public Task AssemblyConfigurationWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration(typeof(ClassBeingTested).Assembly);
        return Verify("Foo", settings);
    }
    [Fact]
    public Task AssemblyConfigurationFluent()
    {
        return Verify("Foo")
            .UniqueForAssemblyConfiguration();
    }

    [Fact]
    public Task AssemblyConfigurationFluentWithAssembly()
    {
        return Verify("Foo")
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
        return Verify(arg, settings);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParametersFluent(string arg)
    {
        return Verify(arg)
            .UseTextForParameters(arg);
    }

    #endregion

    [Fact]
    public Task UseTextForParametersNoParam()
    {
        return Verify("Value")
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
        return Verify("Foo", settings);
    }

    [Fact]
    public Task ArchitectureFluent()
    {
        return Verify("Foo")
            .UniqueForArchitecture();
    }

    [Fact]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("Foo", settings);
    }

    [Fact]
    public Task OSPlatformFluent()
    {
        return Verify("Foo")
            .UniqueForOSPlatform();
    }
}