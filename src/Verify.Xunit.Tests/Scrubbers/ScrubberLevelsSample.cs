using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

#region ScrubberLevelsSampleXunit

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
        return Verifier.Verify("One Two Three", settings);
    }

    [Fact]
    public Task UsageFluent()
    {
        return Verifier.Verify("One Two Three", classLevelSettings)
            .AddScrubber(s => s.Replace("Two", "B"));
    }

    static ScrubberLevelsSample()
    {
        // Should be dont at appdomain startup
        VerifierSettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion