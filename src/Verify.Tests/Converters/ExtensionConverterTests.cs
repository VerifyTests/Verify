[UsesVerify]
public class ExtensionConverterTests
{
    [ModuleInitializer]
    public static void TextSplitInit()
    {
        VerifierSettings.RegisterFileConverter(
            "split",
            (stream, _) => new(null, "txt", stream));
    }

    [Fact]
    public Task TextSplit()
    {
        return Verify(IoHelpers.OpenRead("sample.split"))
            .UseExtension("txt");
    }

    [ModuleInitializer]
    public static void ExtensionConversionStringBuilderInit()
    {
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversionStringBuilder",
            (_, _) => new(null, "txt", new StringBuilder("Foo")));
    }

    [Fact]
    public Task ExtensionConversionStringBuilder()
    {
        return Verify(new MemoryStream())
            .UseExtension("ExtensionConversionStringBuilder");
    }

    [ModuleInitializer]
    public static void ExtensionConversionInit()
    {
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversion",
            (_, _) => new(null, "txt", "Foo"));
    }

    [Fact]
    public Task ExtensionConversion()
    {
        return Verify(new MemoryStream())
            .UseExtension("ExtensionConversion");
    }

    [ModuleInitializer]
    public static void AsyncExtensionConversionInit()
    {
        VerifierSettings.RegisterFileConverter(
            "AsyncExtensionConversion",
            (_, _) => Task.FromResult(new ConversionResult(null, "txt", "Foo")));
    }

    [Fact]
    public Task AsyncExtensionConversion()
    {
        return Verify(new MemoryStream())
            .UseExtension("AsyncExtensionConversion");
    }

    [ModuleInitializer]
    public static void WithInfoInit()
    {
        VerifierSettings.RegisterFileConverter(
            "WithInfo",
            (_, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "txt", "Foo");
            });
    }

    [Fact]
    public Task WithInfo()
    {
        return Verify(new MemoryStream())
            .UseExtension("WithInfo");
    }

    [ModuleInitializer]
    public static void WithInfoAndBinaryInit()
    {
        VerifierSettings.RegisterFileConverter(
            "WithInfoAndBinary",
            (stream, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "png", stream);
            });
    }

    [Fact]
    public async Task WithInfoAndBinary()
    {
        await Verify(File.OpenRead("sample.png"))
            .UseExtension("WithInfoAndBinary");
    }

    [Fact]
    public async Task WithInfoAndModifiedBinary()
    {
        await Verify(File.OpenRead("sample.png"))
            .UseExtension("WithInfoAndBinary")
            .AutoVerify();

        await Assert.ThrowsAsync<VerifyException>(
            () => Verify(File.OpenRead("sample2.png"))
                .UseExtension("WithInfoAndBinary")
                .DisableRequireUniquePrefix().DisableDiff());
        AsserFileExists();
    }

    static void AsserFileExists([CallerFilePath] string sourceFile = "")
    {
        var directory = Path.GetDirectoryName(sourceFile)!;
        var file = Path.Combine(directory, "ExtensionConverterTests.WithInfoAndModifiedBinary.01.received.png");
        Assert.True(File.Exists(file));
    }

    [ModuleInitializer]
    public static void ConflictingExtensionInit()
    {
        VerifierSettings.RegisterFileConverter(
            (_, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "conflictingextension", new MemoryStream());
            },
            (target, extension, context) => target is ClassWithConflictingExtension);
        VerifierSettings.RegisterFileConverter(
            "conflictingextension",
            (_, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "conflictingextension", new MemoryStream());
            });
    }

    [Fact]
    public Task WithConflictingExtension()
    {
        var target = new ClassWithConflictingExtension();
        return ThrowsTask(() => Verify(target)
                .UseExtension("conflictingextension"))
            .UseMethodName("WithConflictingExtensionOuter")
            .IgnoreStackTrack();
    }

    public class ClassWithConflictingExtension
    {
    }
}