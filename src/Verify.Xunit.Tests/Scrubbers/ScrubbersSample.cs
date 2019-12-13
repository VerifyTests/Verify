using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ScrubbersSample :
    VerifyBase
{
    private VerifySettings classLevelSettings;

    [Fact]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    [Fact]
    public Task AfterJson()
    {
        var target = new ToBeScrubbed
        {
            RowVersion = "0x00000000000007D3"
        };

        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("0x00000000000007D3", "TheRowVersion"));
        return Verify(target, settings);
    }

    public ScrubbersSample(ITestOutputHelper output) :
        base(output)
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [GlobalSetUp]
    public static class GlobalSetup
    {
        public static void Setup()
        {
            Global.AddScrubber(s => s.Replace("One", "A"));
        }
    }
}