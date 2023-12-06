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

    [Theory]
    [InlineData("VerifySamplePlugin", "VerifyTests.VerifySamplePlugin")]
    [InlineData("Verify.ICSharpCode.Decompiler", "VerifyTests.VerifyICSharpCodeDecompiler")]
    public void GetTypeName(string assemblyName, string expectedTypeName) =>
        Assert.Equal(expectedTypeName, VerifierSettings.GetTypeName(assemblyName));

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
    public void InvokeInitializeWithOptional()
    {
        VerifierSettings.InvokeInitialize(typeof(InitializeTargetWithOptional));
        Assert.True(InitializeTargetWithOptional.Initialized);
        Assert.True(InitializeTargetWithOptional.Param);
    }

    public static class InitializeTargetWithOptional
    {
        public static bool Initialized { get; set; }
        public static bool Param { get; set; }

        public static void Initialize(bool param = true)
        {
            Param = param;
            Initialized = true;
        }
    }

    [Fact]
    public void InvokeInitializeWithAmbiguous()
    {
        VerifierSettings.InvokeInitialize(typeof(InitializeTargetWithAmbiguous));
        Assert.True(InitializeTargetWithAmbiguous.Initialized);
        Assert.False(InitializeTargetWithAmbiguous.Param);
    }

    public static class InitializeTargetWithAmbiguous
    {
        public static bool Initialized { get; set; }
        public static bool Param { get; set; }

        public static void Initialize() =>
            Initialized = true;

        public static void Initialize(bool param = true) =>
            Param = param;
    }

    [Fact]
    public void InvokeInitializeWithOneNotDefault()
    {
        VerifierSettings.InvokeInitialize(typeof(InitializeWithOneNotDefault));
        Assert.True(InitializeWithOneNotDefault.Initialized);
        Assert.True(InitializeWithOneNotDefault.Param);
    }

    public static class InitializeWithOneNotDefault
    {
        public static bool Initialized { get; set; }
        public static bool Param { get; set; }

        public static void Initialize(int x)
        {
        }

        public static void Initialize(bool param = true)
        {
            Param = param;
            Initialized = true;
        }
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