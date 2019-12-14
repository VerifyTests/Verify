using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class ParametersSample
{
    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task Usage(string arg)
    {
        return Verifier.Verify(arg);
    }
}