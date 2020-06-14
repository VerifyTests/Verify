using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using static VerifyXunit.Verifier;

[UsesVerify]
public class VerifyTextSample
{
    [Fact]
    public Task Simple()
    {
        return Verify("Foo");
    }
}