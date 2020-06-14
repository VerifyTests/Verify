using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTesting;
using VerifyMSTest;

#region ScrubberLevelsSampleMSTest
[TestClass]
public class ScrubberLevelsSample :
    VerifyBase
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
        return Verify("One Two Three", settings);
    }

    [AssemblyInitialize]
    public static void Setup(TestContext testContext)
    {
        SharedVerifySettings.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion