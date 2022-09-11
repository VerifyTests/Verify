﻿[UsesVerify]
public class Tests
{
    #region UseSplitModeForUniqueDirectory

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.UseSplitModeForUniqueDirectory();

    #endregion

    [Fact]
    public Task ValueTest() =>
        Verify("value")
            .UseUniqueDirectory();

    [Fact]
    public Task Target() =>
        Verify(
                "Target",
                new[]
                {
                    new Target("txt", "data")
                })
            .UseUniqueDirectory();

    [Fact]
    public Task TargetWithName() =>
        Verify(
                "Target",
                new[]
                {
                    new Target("txt", "data", "name")
                })
            .UseUniqueDirectory();

    [Fact]
    public Task TyeName() =>
        Verify("TyeName")
            .UseUniqueDirectory()
            .UseTypeName("TheTypeName");

    [Fact]
    public Task UniqueForRuntime() =>
        Verify("UniqueForRuntime")
            .UseUniqueDirectory()
            .UniqueForRuntime();

    [Fact]
    public Task UniqueForTargetFrameworkAndVersion() =>
        Verify("UniqueForTargetFrameworkAndVersion")
            .UseUniqueDirectory()
            .UniqueForTargetFrameworkAndVersion();

    [Fact]
    public Task MethodName() =>
        Verify("MethodName")
            .UseUniqueDirectory()
            .UseMethodName("TheMethodName");

    [Fact]
    public Task Parameter() =>
        Verify("Parameter")
            .UseUniqueDirectory()
            .UseParameters("Parameter");

    [Fact]
    public Task FileName() =>
        Verify("FileName")
            .UseUniqueDirectory()
            .UseFileName("TheFileName");
}