using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class VerifyTextSample
{
    [Test]
    public Task Simple()
    {
        return Verifier.Verify("Foo");
    }
}