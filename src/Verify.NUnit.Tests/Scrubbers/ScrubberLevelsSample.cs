#region ScrubberLevelsSampleNUnit

[TestFixture]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
        classLevelSettings.AddScrubber(new LiteralReplacePatternScrubber("Three", "C", boundaryCheck: false));
    }

    [Test]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(new LiteralReplacePatternScrubber("Two", "B", boundaryCheck: false));
        return Verify("One Two Three", settings);
    }

    [Test]
    public Task SimpleFluent() =>
        Verify("One Two Three", classLevelSettings)
            .AddScrubber(new LiteralReplacePatternScrubber("Two", "B", boundaryCheck: false));

    [ModuleInitializer]
    public static void Setup() =>
        VerifierSettings.AddScrubber(new LiteralReplacePatternScrubber("One", "A", boundaryCheck: false));
}

#endregion