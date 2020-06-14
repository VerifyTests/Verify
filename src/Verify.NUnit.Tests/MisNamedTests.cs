using System.Threading.Tasks;
using NUnit.Framework;
using static VerifyNUnit.Verifier;

[TestFixture]
public class DiffNamedTests
{
    [Test]
    public Task ShouldPass()
    {
        return Verify("Foo");
    }
}