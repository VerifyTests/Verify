[UsesVerify]
public class ExtensionConverterTests
{
    [ModuleInitializer]
    public static void NestedInit()
    {
        VerifierSettings.RegisterFileConverter(
            "level1",
            (stream, _) =>
                new(
                    "level1Info",
                    new List<Target>
                    {
                        new("txt", "text from level1"),
                        new("level2", stream)
                    }));
        VerifierSettings.RegisterFileConverter(
            "level2",
            async (stream, _) =>
                new(
                    "level2Info",
                    new List<Target>
                    {
                        new("txt", "text from level2"),
                        new("txt", await stream.ReadString())
                    }));
    }

    [Fact]
    public Task Nested() =>
        Verify(IoHelpers.OpenRead("sample.level1"));

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
        Verify(new MemoryStream(new byte[]{1}))
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
                        new("txt", "value1"),
                        new("txt", "value2")
                    };
                    return new(null, targets);
                }));

    [Fact]
    public Task ExtensionConversionMultipleTargets() =>
        Verify(new MemoryStream(new byte[]{1}))
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
                        new("txt", "value1", "name1"),
                        new("txt", "value2", "name2")
                    };
                    return new(null, targets);
                }));

    [Fact]
    public Task ExtensionConversionNamedTarget() =>
        Verify(new MemoryStream(new byte[]{1}))
            .UseExtension("ExtensionConversionNamedTarget");

    [ModuleInitializer]
    public static void ExtensionConversionNamedMixedTargetInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversionNamedMixedTarget",
            new Conversion<Stream>(
                (_, _) =>
                {
                    var targets = new Target[]
                    {
                        new("txt", "value1", "name1"),
                        new("txt", "value2")
                    };
                    return new(null, targets);
                }));

    [Fact]
    public Task ExtensionConversionNamedMixedTarget() =>
        Verify(new MemoryStream(new byte[]{1}))
            .UseExtension("ExtensionConversionNamedMixedTarget");

    [ModuleInitializer]
    public static void ExtensionConversionInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversion",
            (_, _) => new(null, "txt", "Foo"));

    [Fact]
    public Task ExtensionConversion() =>
        Verify(new MemoryStream(new byte[]{1}))
            .UseExtension("ExtensionConversion");

    [ModuleInitializer]
    public static void AsyncExtensionConversionInit() =>
        VerifierSettings.RegisterFileConverter(
            "AsyncExtensionConversion",
            (_, _) => Task.FromResult(new ConversionResult(null, "txt", "Foo")));

    [Fact]
    public Task AsyncExtensionConversion() =>
        Verify(new MemoryStream(new byte[]{1}))
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
        Verify(new MemoryStream(new byte[]{1}))
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
    public Task WithInfoAndBinary() =>
        Verify(File.OpenRead("sample.png"))
            .UseExtension("WithInfoAndBinary");

#if NET6_0

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
        var file = CurrentFile.Relative($"ExtensionConverterTests.WithInfoAndModifiedBinary.{Namer.RuntimeAndVersion}.received.png");
        Assert.True(File.Exists(file));
    }

#endif
}