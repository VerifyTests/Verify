#region ScrubberLevelsSampleMSTest

[TestClass]
public partial class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
        classLevelSettings.AddScrubber(new LiteralReplacePatternScrubber("Three", "C", boundaryCheck: false));
    }

    [TestMethod]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(new LiteralReplacePatternScrubber("Two", "B", boundaryCheck: false));
        return Verify("One Two Three", settings);
    }

    [TestMethod]
    public Task SimpleFluent() =>
        Verify("One Two Three", classLevelSettings)
            .AddScrubber(new LiteralReplacePatternScrubber("Two", "B", boundaryCheck: false));

    [AssemblyInitialize]
    public static void Setup(TestContext testContext) =>
        VerifierSettings.AddScrubber(new LiteralReplacePatternScrubber("One", "A", boundaryCheck: false));
}

#endregion