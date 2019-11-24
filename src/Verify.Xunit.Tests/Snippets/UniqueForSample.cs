using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

#region UniqueForSample
public class UniqueForSample :
    VerifyBase
{
    [Fact]
    public Task Runtime()
    {
        UniqueForRuntime();
        return Verify("value");
    }

    [Fact]
    public Task RuntimeAndVersion()
    {
        UniqueForRuntimeAndVersion();
        return Verify("value");
    }

    public UniqueForSample(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endregion