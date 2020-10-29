using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class Tests
{
    [Test]
    public Task VerifyFileContent()
    {
        return Verifier.VerifyFileContent("Foo", "ext");
    }

    [Test]
    public Task VerifyJson()
    {
        return Verifier.VerifyJson("{x:y}");
    }
}