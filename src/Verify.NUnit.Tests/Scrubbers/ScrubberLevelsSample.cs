using System.Threading.Tasks;
using NUnit.Framework;
using Verify;
using VerifyNUnit;

#region ScrubberLevelsSampleNUnit
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
        return Verifier.Verify("One Two Three", settings);
    }

    [OneTimeSetUp]
    public static void Setup()
    {
        SharedVerifySettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion