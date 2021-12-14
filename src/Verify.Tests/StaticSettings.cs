[UsesVerify]
public class StaticSettings
{
    [Fact]
    public Task Test()
    {
        return Verify("String to verify");
    }
}

public static class StaticSettingsUsage
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddScrubber(_ => _.Replace("String to verify", "new value"));
    }
}
