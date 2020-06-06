using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

#region ScrubberLevelsSampleXunit
public class ScrubberLevelsSample :
    VerifyBase
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample(ITestOutputHelper output) :
        base(output)
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [Fact]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    static ScrubberLevelsSample()
    {
        // Should be dont at appdomain startup
        SharedVerifySettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion