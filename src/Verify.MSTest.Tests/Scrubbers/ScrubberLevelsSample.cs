using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;
using VerifyMSTest;

namespace TheTests;

#region ScrubberLevelsSampleMSTest

[TestClass]
public class ScrubberLevelsSample :
    VerifyBase
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
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
    public Task SimpleFluent()
    {
        return Verify("One Two Three", classLevelSettings)
            .AddScrubber(s => s.Replace("Two", "B"));
    }

    [AssemblyInitialize]
    public static void Setup(TestContext testContext)
    {
        VerifierSettings.AddScrubber(s => s.Replace("One", "A"));
    }
}

#endregion