using System.Threading.Tasks;
using VerifyBunit;
using Xunit;
using Xunit.Abstractions;

public class VerifyTextSample :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        return Verify("Foo");
    }

    public VerifyTextSample(ITestOutputHelper output) :
        base(output)
    {
    }
}