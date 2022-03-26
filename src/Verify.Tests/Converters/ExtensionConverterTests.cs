[UsesVerify]
public class ExtensionConverterTests
{
    [ModuleInitializer]
    public static void TextSplitInit() =>
        VerifierSettings.RegisterFileConverter(
            "split",
            (stream, _) => new(null, "txt", stream));

    [Fact]
    public Task TextSplit() =>
        Verify(IoHelpers.OpenRead("sample.split"))
            .UseExtension("txt");

    [ModuleInitializer]
    public static void ExtensionConversionStringBuilderInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversionStringBuilder",
            (_, _) => new(null, "txt", new StringBuilder("Foo")));

    [Fact]
    public Task ExtensionConversionStringBuilder() =>
        Verify(new MemoryStream())
            .UseExtension("ExtensionConversionStringBuilder");

    [ModuleInitializer]
    public static void ExtensionConversionMultipleTargetsInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversionMultipleTargets",
            new Conversion<Stream>(
                (_, _) =>
                {
                    var targets = new Target[]
                    {
                        new("txt", "value1", null),
                        new("txt", "value2", null)
                    };
                    return new(null, targets);
                }));

    [Fact]
    public Task ExtensionConversionMultipleTargets() =>
        Verify(new MemoryStream())
            .UseExtension("ExtensionConversionMultipleTargets");

    [ModuleInitializer]
    public static void ExtensionConversionNamedTargetInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversionNamedTarget",
            new Conversion<Stream>(
                (_, _) =>
                {
                    var targets = new Target[]
                    {
                        new("txt", "value1", null),
                        new("txt", "value2", null)
                    };
                    return new(null, targets);
                }));

    [Fact]
    public Task ExtensionConversionNamedTarget() =>
        Verify(new MemoryStream())
            .UseExtension("ExtensionConversionNamedTarget");

    [ModuleInitializer]
    public static void ExtensionConversionInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversion",
            (_, _) => new(null, "txt", "Foo"));

    [Fact]
    public Task ExtensionConversion() =>
        Verify(new MemoryStream())
            .UseExtension("ExtensionConversion");

    [ModuleInitializer]
    public static void AsyncExtensionConversionInit() =>
        VerifierSettings.RegisterFileConverter(
            "AsyncExtensionConversion",
            (_, _) => Task.FromResult(new ConversionResult(null, "txt", "Foo")));

    [Fact]
    public Task AsyncExtensionConversion() =>
        Verify(new MemoryStream())
            .UseExtension("AsyncExtensionConversion");

    [ModuleInitializer]
    public static void WithInfoInit() =>
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

    [Fact]
    public Task WithInfo() =>
        Verify(new MemoryStream())
            .UseExtension("WithInfo");

    [ModuleInitializer]
    public static void WithInfoAndBinaryInit() =>
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

    [Fact]
    public async Task WithInfoAndBinary() =>
        await Verify(File.OpenRead("sample.png"))
            .UseExtension("WithInfoAndBinary");

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
}