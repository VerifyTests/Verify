public class PluginConventionTests
{
    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.InitializePlugins();

    [Fact]
    public void Find() =>
        Assert.True(VerifySamplePlugin.Initialized);

    [Fact]
    public void TryGetType()
    {
        Assert.True(VerifierSettings.TryGetType(typeof(VerifySamplePlugin).Assembly.Location, out var type));
        Assert.Same(typeof(VerifySamplePlugin), type);
    }
}