using System.Threading.Tasks;
using NUnit.Framework;
using Verify;
using VerifyNUnit;

[TestFixture]
public class ScrubbersSample
{
    VerifySettings classLevelSettings;

    public ScrubbersSample()
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

    [Test]
    public Task AfterJson()
    {
        var target = new ToBeScrubbed
        {
            RowVersion = "0x00000000000007D3"
        };

        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("0x00000000000007D3", "TheRowVersion"));
        return Verifier.Verify(target, settings);
    }

    public static class GlobalSetup
    {
        [OneTimeSetUp]
        public static void Setup()
        {
            Global.AddScrubber(s => s.Replace("One", "A"));
        }
    }
}