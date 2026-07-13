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
    public void TryGetTypeNullAssembly() =>
        Assert.Throws<Exception>(() => VerifierSettings.TryGetType("Verify.BadAssemblyName.dll", out _));

    [Fact]
    public void ReadReferencedFileNames()
    {
        var deps =
            """
            {
              "targets": {
                ".NETCoreApp,Version=v8.0": {
                  "MyApp/1.0.0": {
                    "runtime": {
                      "MyApp.dll": {}
                    }
                  },
                  "Verify.Foo/1.0.0": {
                    "runtime": {
                      "lib/net8.0/Verify.Foo.dll": {}
                    }
                  },
                  "NoRuntimeLibrary/1.0.0": {}
                }
              }
            }
            """;
        var files = VerifierSettings.ReadReferencedFileNames(deps);
        // project reference style key
        Assert.Contains("MyApp.dll", files);
        // package reference style key, with the lib/tfm path stripped
        Assert.Contains("Verify.Foo.dll", files);
        // libraries without a runtime section are ignored
        Assert.Equal(2, files.Count);
    }

    [Fact]
    public void ReferencedAssemblyFilesIncludesReferencedPlugin()
    {
        var referenced = VerifierSettings.GetReferencedAssemblyFiles(typeof(PluginConventionTests).Assembly);
        // No deps.json (for example on .NET Framework), so filtering is not applied.
        if (referenced is null)
        {
            return;
        }

        // Verify.SamplePlugin is an actual reference of this test project.
        Assert.Contains("Verify.SamplePlugin.dll", referenced);
        Assert.DoesNotContain("Verify.StaleUnreferencedTestPlugin.dll", referenced);
    }

    [Fact]
    public void StaleUnreferencedPluginIsSkipped()
    {
        var assembly = typeof(PluginConventionTests).Assembly;
        // Filtering relies on a deps.json (not present on for example .NET Framework).
        if (VerifierSettings.GetReferencedAssemblyFiles(assembly) is null)
        {
            return;
        }

        using var directory = new TempDirectory();
        // A stale plugin assembly left in the output directory but not referenced by the test assembly.
        // The content is intentionally not a real assembly: if it were loaded it would fail.
        File.WriteAllText(directory.BuildPath("Verify.StaleUnreferencedTestPlugin.dll"), "not a real assembly");

        // Should not throw: the stale assembly is not referenced by the test assembly, so it is skipped.
        VerifierSettings.InitializePlugins(directory.Path, assembly);
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