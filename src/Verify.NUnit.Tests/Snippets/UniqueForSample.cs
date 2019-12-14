using System.Threading.Tasks;
using NUnit.Framework;
using Verify;
using VerifyNUnit;

#region UniqueForSample
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
    public Task RuntimeAndVersion()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        return Verifier.Verify("value", settings);
    }
}
#endregion