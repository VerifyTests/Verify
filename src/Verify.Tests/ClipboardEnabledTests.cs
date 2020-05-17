using System;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ClipboardEnabledTests :
    VerifyBase
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
        StringBuilder builder = new StringBuilder();
        builder.Equals(new StringBuilder());
        var exception = Assert.Throws<Exception>(() => ClipboardEnabled.ParseEnvironmentVariable("foo"));
        return Verify(exception);
    }

    public ClipboardEnabledTests(ITestOutputHelper output) :
        base(output)
    {
    }
}