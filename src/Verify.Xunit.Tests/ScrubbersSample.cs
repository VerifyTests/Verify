using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ScrubbersSample :
    VerifyBase
{
    [Fact]
    public async Task Simple()
    {
        AddScrubber(s => s.Replace("Two", "B"));
        await VerifyText("One Two Three");
    }

    public ScrubbersSample(ITestOutputHelper output) :
        base(output)
    {
        AddScrubber(s => s.Replace("Three", "C"));
    }

    static ScrubbersSample()
    {
        Global.AddScrubber(s => s.Replace("One", "A"));
    }
}