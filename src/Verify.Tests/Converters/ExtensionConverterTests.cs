[UsesVerify]
public class ExtensionConverterTests
{
    [ModuleInitializer]
    public static void RecursiveInit() =>
        VerifierSettings.RegisterFileConverter(
            "recursive",
            (stream, _) =>
                new(
                    "recursiveInfo",
                    new List<Target>
                    {
                        new("recursive", stream)
                    }));

    // eg when a converter is getting info from a png
    [Fact]
    public Task Recursive() =>
        Verify(IoHelpers.OpenRead("sample.recursive"));

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
                        new("txt", await stream.ReadStringBuilderWithFixedLines())
                    }));
    }

    [Fact]
    public Task Nested() =>
        Verify(IoHelpers.OpenRead("sample.level1"));

    [Fact]
    public Task NestedTarget()
    {
        var targets = new[]
        {
            new Target("level1", new MemoryStream())
        };
        return Verify(targets);
    }

    [ModuleInitializer]
    public static void TextSplitInit() =>
        VerifierSettings.RegisterFileConverter(
            "split",
            (stream, _) => new(null, "txt", stream));

    [Fact]
    public Task TextSplit() =>
        Verify(IoHelpers.OpenRead("sample.split"), "txt");

    [ModuleInitializer]
    public static void ExtensionConversionStringBuilderInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversionStringBuilder",
            (_, _) => new(null, "txt", new StringBuilder("Foo")));

    [Fact]
    public Task ExtensionConversionStringBuilder() =>
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "ExtensionConversionStringBuilder");

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
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "ExtensionConversionMultipleTargets");

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
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "ExtensionConversionNamedTarget");

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
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "ExtensionConversionNamedMixedTarget");

    [ModuleInitializer]
    public static void ExtensionConversionInit() =>
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversion",
            (_, _) => new(null, "txt", "Foo"));

    [Fact]
    public Task ExtensionConversion() =>
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "ExtensionConversion");

    [ModuleInitializer]
    public static void AsyncExtensionConversionInit() =>
        VerifierSettings.RegisterFileConverter(
            "AsyncExtensionConversion",
            (_, _) => Task.FromResult(new ConversionResult(null, "txt", "Foo")));

    [Fact]
    public Task AsyncExtensionConversion() =>
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "AsyncExtensionConversion");

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
        Verify(new MemoryStream(new byte[]
        {
            1
        }), "WithInfo");

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
        Verify(File.OpenRead("sample.png"), "WithInfoAndBinary");

#if NET6_0_OR_GREATER

    [Fact]
    public async Task WithInfoAndModifiedBinary()
    {
        await Verify(File.OpenRead("sample.png"), "WithInfoAndBinary")
            .AutoVerify();

        await Assert.ThrowsAsync<VerifyException>(
            () => Verify(File.OpenRead("sample2.png"), "WithInfoAndBinary")
                .DisableRequireUniquePrefix().DisableDiff());
        var file = CurrentFile.Relative($"ExtensionConverterTests.WithInfoAndModifiedBinary.{Namer.RuntimeAndVersion}.received.png");
        Assert.True(File.Exists(file));
    }

#endif
}