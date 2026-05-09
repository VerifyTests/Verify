#region ScrubberLevelsSampleXunit

public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
        classLevelSettings.AddScrubber(new LiteralReplacePatternScrubber("Three", "C", boundaryCheck: false));
    }

    [Fact]
    public Task Usage()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(new LiteralReplacePatternScrubber("Two", "B", boundaryCheck: false));
        return Verify("One Two Three", settings);
    }

    [Fact]
    public Task UsageFluent() =>
        Verify("One Two Three", classLevelSettings)
            .AddScrubber(new LiteralReplacePatternScrubber("Two", "B", boundaryCheck: false));

    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.AddScrubber(new LiteralReplacePatternScrubber("One", "A", boundaryCheck: false));
}

#endregion