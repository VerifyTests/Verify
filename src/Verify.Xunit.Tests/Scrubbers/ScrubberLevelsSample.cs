using System.Threading.Tasks;
using Verify;
using VerifyXunit;

#region ScrubberLevelsSampleXunit
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [VerifyFact]
    public Task Usage()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verifier.Verify("One Two Three", settings);
    }

    static ScrubberLevelsSample()
    {
        // Should be dont at appdomain startup
        SharedVerifySettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion