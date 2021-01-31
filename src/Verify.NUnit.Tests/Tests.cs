using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class Tests
{
    [TestCase("Value1")]
    public Task UseFileNameWithParam(string arg)
    {
        return Verifier.Verify(arg)
            .UseFileName("UseFileNameWithParam");
    }
}