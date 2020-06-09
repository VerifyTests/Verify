using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

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
        var exception = Assert.Throws<Exception>(() => ClipboardEnabled.ParseEnvironmentVariable("foo"));
        return Verifier.Verify(exception);
    }
}