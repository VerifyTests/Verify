using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public abstract class Base
{
    [Test]
    public Task TestInBase()
    {
        return Verifier.Verify("Foo");
    }

    [Test]
    public virtual Task TestToOverride()
    {
        return Verifier.Verify("Foo");
    }
}