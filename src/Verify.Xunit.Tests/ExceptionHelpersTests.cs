using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public class ExceptionHelpersTests :
    VerifyBase
{
    [Fact]
    public async Task Message()
    {
        var exception = Assert.Throws<EqualException>(() => Assert.Equal("A", "b"));
        await Verify(exception.Message);
    }

    public ExceptionHelpersTests(ITestOutputHelper output) :
        base(output)
    {
    }
}