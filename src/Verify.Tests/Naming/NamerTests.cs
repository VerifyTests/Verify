[UsesVerify]
public class NamerTests
{
#if NET6_0_OR_GREATER && DEBUG
    // [Fact]
    // public async Task ThrowOnConflict()
    // {
    //     static Task Run()
    //     {
    //         return Verify("Value")
    //             .UseMethodName("Conflict1")
    //             .DisableDiff();
    //     }
    //
    //     try
    //     {
    //         await Run();
    //     }
    //     catch
    //     {
    //     }
    //
    //     await ThrowsTask(Run)
    //         .ScrubLinesContaining("InnerVerifier.ValidatePrefix")
    //         .UseMethodName("ThrowOnConflict")
    //         .AddScrubber(_ => _.Replace(@"\", "/"));
    // }

    [Fact]
    public async Task DoesntThrowOnConflict()
    {
        static Task Run()
        {
            return Verify("ValueDoesntThrowOnConflict")
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

        await Verify("ValueDoesntThrowOnConflict")
            .UseMethodName("DoesntThrowOnConflict")
            .AddScrubber(_ => _.Replace(@"\", "/"));
    }
#endif

    #region MultipleCalls

    [Fact]
    public Task MultipleCalls() =>
        Task.WhenAll(
            Verify("Value1MultipleCalls")
                .UseMethodName("MultipleCalls_1"),
            Verify("Value1MultipleCalls")
                .UseMethodName("MultipleCalls_2"));

    #endregion

    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify(Namer.Runtime, settings);
    }

    [Fact]
    public Task RuntimeFluent() =>
        Verify(Namer.Runtime)
            .UniqueForRuntime();

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify(Namer.RuntimeAndVersion, settings);
    }

    [Fact]
    public Task RuntimeAndVersionFluent() =>
        Verify(Namer.RuntimeAndVersion)
            .UniqueForRuntimeAndVersion();

    [Fact]
    public Task TargetFramework()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFramework();
        return Verify("FooTargetFramework", settings);
    }

    [Fact]
    public Task TargetFrameworkWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFramework(typeof(ClassBeingTested).Assembly);
        return Verify("FooTargetFrameworkWithAssembly", settings);
    }

    [Fact]
    public Task TargetFrameworkFluent() =>
        Verify("FooTargetFrameworkFluent")
            .UniqueForTargetFramework();

    [Fact]
    public Task TargetFrameworkFluentWithAssembly() =>
        Verify("FooTargetFrameworkFluentWithAssembly")
            .UniqueForTargetFramework(typeof(ClassBeingTested).Assembly);

    [Fact]
    public Task TargetFrameworkAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        return Verify("FooTargetFrameworkAndVersion", settings);
    }

    [Fact]
    public Task TargetFrameworkAndVersionWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion(typeof(ClassBeingTested).Assembly);
        return Verify("FooTargetFrameworkAndVersionWithAssembly", settings);
    }

    [Fact]
    public Task TargetFrameworkAndVersionFluent() =>
        Verify("FooTargetFrameworkAndVersionFluent")
            .UniqueForTargetFrameworkAndVersion();

    [Fact]
    public Task TargetFrameworkAndVersionFluentWithAssembly() =>
        Verify("FooTargetFrameworkAndVersionFluentWithAssembly")
            .UniqueForTargetFrameworkAndVersion(typeof(ClassBeingTested).Assembly);

    [Fact]
    public async Task UseFileName()
    {
        #region UseFileName

        var settings = new VerifySettings();
        settings.UseFileName("CustomFileName");
        await Verify("valueUseFileName", settings);

        #endregion
    }

    [Fact]
    public Task UseFileNameWithUnique()
    {
        var settings = new VerifySettings();
        settings.UseFileName("CustomFileNameWithUnique");
        settings.UniqueForRuntime();
        return Verify("valueUseFileNameWithUnique", settings);
    }

    [Fact]
    public async Task UseFileNameFluent()
    {
        #region UseFileNameFluent

        await Verify("valueUseFileNameFluent")
            .UseFileName("CustomFileNameFluent");

        #endregion
    }

    [Fact]
    public async Task UseDirectory()
    {
        #region UseDirectory

        var settings = new VerifySettings();
        settings.UseDirectory("CustomDirectory");
        await Verify("valueUseDirectory", settings);

        #endregion
    }

    [Fact]
    public async Task UseDirectoryFluent()
    {
        #region UseDirectoryFluent

        await Verify("valueUseDirectoryFluent")
            .UseDirectory("CustomDirectory");

        #endregion
    }

    [Fact]
    public async Task UseUniqueDirectory()
    {
        #region UseUniqueDirectory

        var settings = new VerifySettings();
        settings.UseUniqueDirectory();
        await Verify("TheValue", settings);

        #endregion
    }

    [Fact]
    public async Task UseUniqueDirectoryFluent()
    {
        #region UseUniqueDirectoryFluent

        await Verify("TheValue")
            .UseUniqueDirectory();

        #endregion
    }

    [Fact]
    public Task UseUniqueDirectory_Target() =>
        Verify(
                "UseUniqueDirectory_Target",
                new[]
                {
                    new Target("txt", "data")
                })
            .UseUniqueDirectory();

    [Fact]
    public Task UseUniqueDirectory_TargetWithName() =>
        Verify(
                "UseUniqueDirectory_Target",
                new[]
                {
                    new Target("txt", "data", "name")
                })
            .UseUniqueDirectory();

    [Fact]
    public Task UseUniqueDirectory_TyeName() =>
        Verify("UseUniqueDirectory_TyeName")
            .UseUniqueDirectory()
            .UseTypeName("TheTypeName");

    [Fact]
    public Task UseUniqueDirectory_UniqueForRuntime() =>
        Verify("UseUniqueDirectory_UniqueForRuntime")
            .UseUniqueDirectory()
            .UniqueForRuntime();

    [Fact]
    public Task UseUniqueDirectory_UniqueForTargetFrameworkAndVersion() =>
        Verify("UseUniqueDirectory_UniqueForTargetFrameworkAndVersion")
            .UseUniqueDirectory()
            .UniqueForTargetFrameworkAndVersion();

    [Fact]
    public Task UseUniqueDirectory_MethodName() =>
        Verify("UseUniqueDirectory_MethodName")
            .UseUniqueDirectory()
            .UseMethodName("TheMethodName");

    [Fact]
    public Task UseUniqueDirectory_Parameter() =>
        Verify("UseUniqueDirectory_Parameter")
            .UseUniqueDirectory()
            .UseParameters("Parameter");

    [Fact]
    public Task UseUniqueDirectory_FileName() =>
        Verify("UseUniqueDirectory_FileName")
            .UseUniqueDirectory()
            .UseFileName("TheFileName");

    [Theory]
    [InlineData("Value1")]
    public async Task UseTooManyParameters(string param1)
    {
        var exception = await Assert.ThrowsAsync<Exception>(async () => await Verify("UseTooManyParameters")
            .UseParameters("param1", "param2"));
        Assert.Equal("The number of passed in parameters (2) must be fewer than the number of parameters for the method (1).", exception.Message);
    }

    [Fact]
    public async Task UseTypeName()
    {
        #region UseTypeName

        var settings = new VerifySettings();
        settings.UseTypeName("CustomTypeName");
        await Verify("valueUseTypeName", settings);

        #endregion
    }

    [Fact]
    public async Task UseTypeNameFluent()
    {
        #region UseTypeNameFluent

        await Verify("valueUseTypeNameFluent")
            .UseTypeName("CustomTypeName");

        #endregion
    }

    [Fact]
    public async Task UseMethodName()
    {
        #region UseMethodName

        var settings = new VerifySettings();
        settings.UseMethodName("CustomMethodName");
        await Verify("valueUseMethodName", settings);

        #endregion
    }

    [Fact]
    public async Task UseMethodNameFluent()
    {
        #region UseMethodNameFluent

        await Verify("valueUseMethodNameFluent")
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
        return Verify("FooAssemblyConfiguration", settings);
    }

    [Fact]
    public Task AssemblyConfigurationWithAssembly()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration(typeof(ClassBeingTested).Assembly);
        return Verify("FooAssemblyConfigurationWithAssembly", settings);
    }

    [Fact]
    public Task AssemblyConfigurationFluent() =>
        Verify("FooAssemblyConfigurationFluent")
            .UniqueForAssemblyConfiguration();

    [Fact]
    public Task AssemblyConfigurationFluentWithAssembly() =>
        Verify("FooAssemblyConfigurationFluentWithAssembly")
            .UniqueForAssemblyConfiguration(typeof(ClassBeingTested).Assembly);

    #region UseTextForParameters

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(arg);
        return Verify(arg + "UseTextForParameters", settings);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParametersFluent(string arg) =>
        Verify(arg + "UseTextForParametersFluent")
            .UseTextForParameters(arg);

    #endregion

    [Fact]
    public Task UseTextForParametersNoParam() =>
        Verify("ValueUseTextForParametersNoParam")
            .UseTextForParameters("Suffix");

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
        return Verify("FooArchitecture", settings);
    }

    [Fact]
    public Task ArchitectureFluent() =>
        Verify("FooArchitectureFluent")
            .UniqueForArchitecture();

    [Fact]
    public Task OSPlatform()
    {
        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();
        return Verify("FooOSPlatform", settings);
    }

    [Fact]
    public Task OSPlatformFluent() =>
        Verify("FooOSPlatformFluent")
            .UniqueForOSPlatform();

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public Task MultipleParams(bool a, bool b)
    {
        var settings = new VerifySettings();
        settings.UseParameters(a, b);
        return Verify("contentMultipleParams", settings);
    }

    #region IgnoreParametersForVerified

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified(arg);
        return Verify("valueIgnoreParametersForVerified", settings);
    }

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("valueIgnoreParametersForVerifiedFluent")
            .IgnoreParametersForVerified(arg);

    #endregion

    [Theory]
    [InlineData("One")]
    [InlineData("foo", "bar", "baz")]
    public Task TheoryWithArray(params string[] values) =>
        Verify("valueTheoryWithArray")
            .UseParameters(values);

    [Theory]
    [InlineData("The<Value")]
    public Task ParametersWithBadPathChars(string value) =>
        Verify(value)
            .UseParameters(value);

    [Fact]
    public Task IgnoreParametersForVerified()
    {
        // note that this test 'generates' the same verified and received filenames as the parameterized method
        var settings = new VerifySettings();
        return Verify("valueIgnoreParametersForVerified", settings);
    }

    [Fact]
    public Task SingleTarget() =>
        Verify(
            new[]
            {
                new Target("txt", "data")
            });

    [Fact]
    public Task SingleTargetWithName() =>
        Verify(
            new[]
            {
                new Target("txt", "data", "theNameA")
            });

    [Fact]
    public Task MultipleTarget() =>
        Verify(
            new[]
            {
                new Target("txt", "data"),
                new Target("txt", "data")
            });

    [Fact]
    public Task MultipleTargetWithName() =>
        Verify(
            new[]
            {
                new Target("txt", "data", "theNameA"),
                new Target("txt", "data", "theNameB")
            });

    [Fact]
    public Task MultipleTargetWithDuplicateName() =>
        Verify(
            new[]
            {
                new Target("txt", "data", "theNameA"),
                new Target("txt", "data", "theNameA")
            });
}