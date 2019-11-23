using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Scrubbers :
    VerifyBase
{
    [Fact]
    public async Task Simple()
    {
        AddScrubber(s => s.Replace("Two", "B"));
        await Verify("One Two Three");
    }

    public Scrubbers(ITestOutputHelper output) :
        base(output)
    {
        AddScrubber(s => s.Replace("Three", "C"));
    }

    static Scrubbers()
    {
        Global.AddScrubber(s => s.Replace("One", "A"));
    }
}