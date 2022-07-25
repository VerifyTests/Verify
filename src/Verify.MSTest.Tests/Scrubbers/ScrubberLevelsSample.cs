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
        classLevelSettings.AddScrubber(_ => _.Replace("Three", "C"));
    }

    [TestMethod]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(_ => _.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    [TestMethod]
    public Task SimpleFluent() =>
        Verify("One Two Three", classLevelSettings)
            .AddScrubber(_ => _.Replace("Two", "B"));

    [AssemblyInitialize]
    public static void Setup(TestContext testContext) =>
        VerifierSettings.AddScrubber(_ => _.Replace("One", "A"));
}

#endregion