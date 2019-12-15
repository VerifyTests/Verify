using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;
using VerifyMSTest;

#region ScrubbersSampleMSTest
[TestClass]
public class ScrubbersSample :
    VerifyBase
{
    VerifySettings classLevelSettings;

    public ScrubbersSample()
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

    [TestMethod]
    public Task AfterJson()
    {
        var target = new ToBeScrubbed
        {
            RowVersion = "0x00000000000007D3"
        };

        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("0x00000000000007D3", "TheRowVersion"));
        return Verify(target, settings);
    }

    [AssemblyInitialize]
    public static void Setup(TestContext testContext)
    {
        Global.AddScrubber(s => s.Replace("One", "A"));
    }
}
#endregion