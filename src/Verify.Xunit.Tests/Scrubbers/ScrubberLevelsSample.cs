#region ScrubberLevelsSampleXunit

[UsesVerify]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
        classLevelSettings.AddScrubber(_ => _.Replace("Three", "C"));
    }

    [Fact]
    public Task Usage()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(_ => _.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    [Fact]
    public Task UsageFluent() =>
        Verify("One Two Three", classLevelSettings)
            .AddScrubber(_ => _.Replace("Two", "B"));

    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.AddScrubber(_ => _.Replace("One", "A"));
}

#endregion