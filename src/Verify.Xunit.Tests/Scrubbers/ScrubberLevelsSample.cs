#region ScrubberLevelsSampleXunit

[UsesVerify]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
        classLevelSettings.AddScrubber(s => s.Replace("Three", "C"));
    }

    [Fact]
    public Task Usage()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(s => s.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    [Fact]
    public Task UsageFluent()
    {
        return Verify("One Two Three", classLevelSettings)
            .AddScrubber(s => s.Replace("Two", "B"));
    }

    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddScrubber(s => s.Replace("One", "A"));
    }
}

#endregion