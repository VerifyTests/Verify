using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

public class VerifyTextSample
{
    [Fact]
    public Task Simple()
    {
        return Verifier.Verify("Foo");
    }
}