using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

#region ScrubberLevelsSampleXunit

using static VerifyXunit.Verifier;

[UsesVerify]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [Fact]
    public Task Usage()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    static ScrubberLevelsSample()
    {
        // Should be dont at appdomain startup
        VerifierSettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion