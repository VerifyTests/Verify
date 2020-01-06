using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ExtensionsTests :
    VerifyBase
{
    [Fact]
    public void IsTextFile()
    {
        Assert.True(Extensions.IsTextFile("file.txt"));
        Assert.False(Extensions.IsTextFile("file.bin"));
    }

    public ExtensionsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}