using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        return Verify("Foo");
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}