using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;
using VerifyTests;

#region UniqueForSampleNUnit

[TestFixture]
public class UniqueForSample
{
    [Test]
    public Task Runtime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task RuntimeFluent()
    {
        return Verifier.Verify("value")
            .UniqueForRuntime();
    }

    [Test]
    public Task AssemblyConfiguration()
    {
        var settings = new VerifySettings();
        settings.UniqueForAssemblyConfiguration();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task AssemblyConfigurationFluent()
    {
        return Verifier.Verify("value")
            .UniqueForAssemblyConfiguration();
    }

    [Test]
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify("value", settings);
    }

    [Test]
    public Task RuntimeAndVersionFluent()
    {
        return Verifier.Verify("value")
            .UniqueForRuntimeAndVersion();
    }
}

#endregion