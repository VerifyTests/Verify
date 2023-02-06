
#pragma warning disable CS0618
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

    [Fact]
    public void InvokeInitialize()
    {
        VerifierSettings.InvokeInitialize(typeof(InitializeTarget));
        Assert.True(InitializeTarget.Initialized);
    }

    public static class InitializeTarget
    {
        public static bool Initialized { get; private set; }

        public static void Initialize() =>
            Initialized = true;
    }

    [Fact]
    public void InvokeInitialized()
    {
        Assert.False(VerifierSettings.GetInitialized(typeof(InitializedTarget)));
        InitializedTarget.Initialized = true;
        Assert.True(VerifierSettings.GetInitialized(typeof(InitializedTarget)));
    }

    public static class InitializedTarget
    {
        public static bool Initialized { get; set; }
    }
}