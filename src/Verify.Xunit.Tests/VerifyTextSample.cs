using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class VerifyTextSample :
    VerifyBase
{
    [Fact]
    public async Task Simple()
    {
        await VerifyText("Foo");
    }

    public VerifyTextSample(ITestOutputHelper output) :
        base(output)
    {
    }
}