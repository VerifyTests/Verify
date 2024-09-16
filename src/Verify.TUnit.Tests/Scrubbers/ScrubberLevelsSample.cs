#region ScrubberLevelsSampleNUnit

[TestFixture]
public class ScrubberLevelsSample
{
    VerifySettings classLevelSettings;

    public ScrubberLevelsSample()
    {
        classLevelSettings = new();
        classLevelSettings.AddScrubber(_ => _.Replace("Three", "C"));
    }

    [Test]
    public Task Simple()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.AddScrubber(_ => _.Replace("Two", "B"));
        return Verify("One Two Three", settings);
    }

    [Test]
    public Task SimpleFluent() =>
        Verify("One Two Three", classLevelSettings)
            .AddScrubber(_ => _.Replace("Two", "B"));

    [ModuleInitializer]
    public static void Setup() =>
        VerifierSettings.AddScrubber(_ => _.Replace("One", "A"));
}

#endregion