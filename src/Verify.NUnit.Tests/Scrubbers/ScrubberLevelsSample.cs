using System.Threading.Tasks;
using NUnit.Framework;
using VerifyTests;

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

    [Test]
    public Task SimpleFluent()
    {
        return Verify("One Two Three", classLevelSettings)
            .AddScrubber(s => s.Replace("Two", "B"));
    }

    [OneTimeSetUp]
    public static void Setup()
    {
        VerifierSettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion