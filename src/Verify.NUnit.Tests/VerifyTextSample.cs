using System.Threading.Tasks;
using NUnit.Framework;
using static VerifyNUnit.Verifier;

[TestFixture]
public class VerifyTextSample
{
    [Test]
    public Task Simple()
    {
        return Verify("Foo");
    }
}