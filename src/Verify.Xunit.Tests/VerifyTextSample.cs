using System.Threading.Tasks;
using VerifyXunit;

public class VerifyTextSample
{
    [VerifyFact]
    public Task Simple()
    {
        return Verifier.Verify("Foo");
    }
}