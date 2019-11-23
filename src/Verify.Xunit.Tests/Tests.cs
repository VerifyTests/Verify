using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    VerifyBase
{
    [Fact]
    public async Task Simple()
    {
        await VerifyText("Foo");
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}