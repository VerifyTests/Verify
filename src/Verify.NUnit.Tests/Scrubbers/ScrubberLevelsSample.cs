using System.Threading.Tasks;
using NUnit.Framework;
using VerifyTesting;

#region ScrubberLevelsSampleNUnit
using static VerifyNUnit.Verifier;

[TestFixture]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [Test]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    [OneTimeSetUp]
    public static void Setup()
    {
        SharedVerifySettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion