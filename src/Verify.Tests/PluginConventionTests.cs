public class PluginConventionTests
{
    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.InitializePlugins();

    [Fact]
    public void Find() =>
        Assert.True(VerifySamplePlugin.Initialized);
}