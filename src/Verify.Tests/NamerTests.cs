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
        return Verify(Namer.runtime, settings);
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify(Namer.runtimeAndVersion, settings);
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