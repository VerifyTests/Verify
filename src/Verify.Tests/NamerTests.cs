using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class NamerTests
{
    [Fact]
    public Task Runtime()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntime();
        return Verifier.Verify(Namer.Runtime, settings);
    }

    [Fact]
    public async Task DeriveTestDirectory()
    {
        string? receivedSourceFile = null;
        string? receivedProjectDirectory = null;
        VerifierSettings.DeriveTestDirectory(
            (sourceFile, projectDirectory) =>
            {
                receivedSourceFile = sourceFile;
                receivedProjectDirectory = projectDirectory;
                return Path.GetDirectoryName(sourceFile);
            });
        await Verifier.Verify("DeriveTestDirectory");
        Assert.NotNull(receivedSourceFile);
        Assert.True(Directory.Exists(receivedProjectDirectory));
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        VerifySettings settings = new();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify(Namer.RuntimeAndVersion, settings);
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
}