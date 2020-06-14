using System.Threading.Tasks;
using NUnit.Framework;
using VerifyTests;

#region UniqueForSampleNUnit
using static VerifyNUnit.Verifier;

[TestFixture]
public class UniqueForSample
{
    [Test]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verify("value", settings);
    }

    [Test]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verify("value", settings);
    }

    [Test]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verify("value", settings);
    }
}
#endregion