using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class VerifyTextSample
{
    [Fact]
    public Task Simple()
    {
        return Verifier.Verify("Foo");
    }
}