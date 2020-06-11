using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;

public class NamerTests
{
    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify(Namer.Runtime, settings);
    }

    [Fact]
    public async Task DeriveTestDirectory()
    {
        string? receivedSourceFile = null;
        string? receivedProjectDirectory = null;
        SharedVerifySettings.DeriveTestDirectory(
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
        var settings = new VerifySettings();
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
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("Foo", settings);
    }
}