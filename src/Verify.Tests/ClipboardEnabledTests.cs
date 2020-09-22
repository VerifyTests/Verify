using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ClipboardEnabledTests
{
    [Fact]
    public void ParseEnvironmentVariable()
    {
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable(null));
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable("false"));
        Assert.True(ClipboardEnabled.ParseEnvironmentVariable("true"));
    }

    [Fact]
    public Task ParseEnvironmentVariable_failure()
    {
        return Verifier.Throws(() => ClipboardEnabled.ParseEnvironmentVariable("foo"));
    }
}