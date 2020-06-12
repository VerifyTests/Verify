using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class DiffNamedTests
{
    [Test]
    public Task ShouldPass()
    {
        return Verifier.Verify("Foo");
    }
}