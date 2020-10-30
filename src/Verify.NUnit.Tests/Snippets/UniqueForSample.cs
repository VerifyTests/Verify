using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

#region UniqueForSampleNUnit

[TestFixture]
public class UniqueForSample
{
    [Test]
    public Task Runtime()
    {
        return Verifier.Verify("value")
            .UniqueForRuntime();
    }

    [Test]
    public Task AssemblyConfiguration()
    {
        return Verifier.Verify("value")
            .UniqueForAssemblyConfiguration();
    }

    [Test]
    public Task RuntimeAndVersion()
    {
        return Verifier.Verify("value")
            .UniqueForRuntimeAndVersion();
    }
}
#endregion