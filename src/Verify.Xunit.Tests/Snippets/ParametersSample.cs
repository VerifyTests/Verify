using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ParametersSample :
    VerifyBase
{
    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task Usage(string arg)
    {
        return Verify(arg);
    }

    public ParametersSample(ITestOutputHelper output) :
        base(output)
    {
    }
}