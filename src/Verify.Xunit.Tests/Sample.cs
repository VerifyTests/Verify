using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Sample :
    VerifyBase
{
    [Fact]
    public async Task Simple()
    {
        var extension = Path.GetExtension("file.txt");
        await Verify("Foo");
    }

    public Sample(ITestOutputHelper output) :
        base(output)
    {
    }
}