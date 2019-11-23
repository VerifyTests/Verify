using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ScrubbersSample :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        AddScrubber(s => s.Replace("Two", "B"));
        return Verify("One Two Three");
    }

    [Fact]
    public Task AfterJson()
    {
        var target = new ToBeScrubbed
        {
            RowVersion = "0x00000000000007D3"
        };

        AddScrubber(s => s.Replace("0x00000000000007D3", "TheRowVersion"));
        return Verify(target);
    }

    public ScrubbersSample(ITestOutputHelper output) :
        base(output)
    {
        AddScrubber(s => s.Replace("Three", "C"));
    }

    public static class ModuleInitializer
    {
        public static void Initialize()
        {
            Global.AddScrubber(s => s.Replace("One", "A"));
        }
    }
}