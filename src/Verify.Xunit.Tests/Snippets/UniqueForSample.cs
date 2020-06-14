using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;

#region UniqueForSampleXunit
[UsesVerify]
public class UniqueForSample
{
    [Fact]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify("value", settings);
    }

    [Fact]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("value", settings);
    }
}
#endregion