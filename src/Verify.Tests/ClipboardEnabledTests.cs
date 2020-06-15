using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Sdk;

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
        var exception = Assert.Throws<XunitException>(() => ClipboardEnabled.ParseEnvironmentVariable("foo"));
        return Verifier.Verify(exception);
    }
}