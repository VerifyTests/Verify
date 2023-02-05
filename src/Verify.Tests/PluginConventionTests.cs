public class PluginConventionTests :
    XunitContextBase
{
 //   [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.InitializePlugins();

    [Fact]
    public void Find()
    {
#pragma warning disable CS0618
        Assert.Same(typeof(VerifySamplePlugin).Assembly, Assembly.LoadWithPartialName("Verify.SamplePlugin"));
#pragma warning restore CS0618
        
        var method = typeof(VerifySamplePlugin).GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public)!;
        
        method.Invoke(null, null);
        Assert.True(VerifySamplePlugin.Initialized);
    }
    //
    // [Fact]
    // public void TryGetType()
    // {
    //     Assert.True(VerifierSettings.TryGetType(typeof(VerifySamplePlugin).Assembly.Location, out var type));
    //     Assert.Same(typeof(VerifySamplePlugin), type);
    // }
    //
    // [Fact]
    // public void InvokeInitialize()
    // {
    //     VerifierSettings.InvokeInitialize(typeof(InitializeTarget));
    //     Assert.True(InitializeTarget.Initialized);
    // }
    //
    // public static class InitializeTarget
    // {
    //     public static bool Initialized { get; private set; }
    //
    //     public static void Initialize() =>
    //         Initialized = true;
    // }
    //
    // [Fact]
    // public void InvokeInitialized()
    // {
    //     Assert.False(VerifierSettings.GetInitialized(typeof(InitializedTarget)));
    //     InitializedTarget.Initialized = true;
    //     Assert.True(VerifierSettings.GetInitialized(typeof(InitializedTarget)));
    // }

    public static class InitializedTarget
    {
        public static bool Initialized { get; set; }
    }
    public PluginConventionTests(ITestOutputHelper output) :
        base(output)
    {
    }
}