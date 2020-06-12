using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;
using VerifyMSTest;

#region ScrubberLevelsSampleMSTest
[TestClass]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [TestMethod]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verifier.Verify("One Two Three", settings);
    }

    [AssemblyInitialize]
    public static void Setup(TestContext testContext)
    {
        SharedVerifySettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion