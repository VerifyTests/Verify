using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

#region UniqueForSampleXunit
using static VerifyXunit.Verifier;

[UsesVerify]
public class UniqueForSample
{
    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }

    [Fact]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }
}
#endregion