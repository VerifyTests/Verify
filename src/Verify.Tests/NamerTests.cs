using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class NamerTests :
    VerifyBase
{
    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify(Namer.Runtime, settings);
    }

    [Fact]
    public async Task DeriveTestDirectory()
    {
        string? received = null;
        SharedVerifySettings.DeriveTestDirectory(
            directory =>
            {
                received = directory;
                return directory;
            });
        await Verify("DeriveTestDirectory");
        Assert.NotNull(received);
        Assert.NotEmpty(received);
        Assert.True(Directory.Exists(received));
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify(Namer.RuntimeAndVersion, settings);
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

    public NamerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}