public class ExtensionConverterTests
{
    [ModuleInitializer]
    public static void RecursiveInit() =>
        VerifierSettings.RegisterStreamConverter(
            "recursive",
            (_, _, _) =>
                new(
                    "recursiveInfo",
                    [
                        new("recursive", new MemoryStream([1])),
                        new("recursive", new MemoryStream([2]))
                    ]));

    // eg when a converter is getting info from a png
    [Fact]
    public Task Recursive() =>
        Verify(IoHelpers.OpenRead("sample.recursive"));

    [ModuleInitializer]
    public static void NestedInit()
    {
        VerifierSettings.RegisterStreamConverter(
            "level1",
            (_, stream, _) =>
                new(
                    "level1Info",
                    [
                        new("txt", "text from level1"),
                        new("level2", stream)
                    ]));
        VerifierSettings.RegisterStreamConverter(
            "level2",
            async (_, stream, _) =>
                new(
                    "level2Info",
                    [
                        new("txt", "text from level2"),
                        new("txt", await stream.ReadStringBuilderWithFixedLines())
                    ]));
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
        VerifierSettings.RegisterStreamConverter(
            "split",
            (_, stream, _) => new(null, "txt", stream));

    [Fact]
    public Task TextSplit() =>
        Verify(IoHelpers.OpenRead("sample.split"), "txt");

    [ModuleInitializer]
    public static void InitEnsureInputs() =>
        VerifierSettings.RegisterStreamConverter(
            "EnsureInputs",
            (name, stream, context) =>
            {
                Assert.Equal("name", name);
                Assert.NotNull(stream);
                Assert.NotNull(context);
                return new(null, "txt", "content");
            });

    [Fact]
    public Task EnsureInputs()
    {
        var stream = new MemoryStream([1]);
        return Verify(new Target("EnsureInputs", stream, "name"));
    }

    [Fact]
    public Task InvalidFileCharactersInName() =>
        Verify(new Target("txt", "value", "name*a?b|c"));

    [ModuleInitializer]
    public static void ExtensionConversionStringBuilderInit() =>
        VerifierSettings.RegisterStreamConverter(
            "ExtensionConversionStringBuilder",
            (_, _, _) => new(null, "txt", new StringBuilder("Foo")));

    [Fact]
    public Task ExtensionConversionStringBuilder() =>
        Verify(new MemoryStream([1]),
            "ExtensionConversionStringBuilder");

    [ModuleInitializer]
    public static void ExtensionConversionMultipleTargetsInit() =>
        VerifierSettings.RegisterStreamConverter(
            "ExtensionConversionMultipleTargets",
            new StreamConversion(
                (_, _, _) =>
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
        Verify(
            new MemoryStream(
            [
                1
            ]),
            "ExtensionConversionMultipleTargets");

    [ModuleInitializer]
    public static void ExtensionConversionNamedTargetInit() =>
        VerifierSettings.RegisterStreamConverter(
            "ExtensionConversionNamedTarget",
            new StreamConversion(
                (_, _, _) =>
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
        Verify(new MemoryStream([1]), "ExtensionConversionNamedTarget");

    [ModuleInitializer]
    public static void ExtensionConversionNamedMixedTargetInit() =>
        VerifierSettings.RegisterStreamConverter(
            "ExtensionConversionNamedMixedTarget",
            new StreamConversion(
                (_, _, _) =>
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
        Verify(new MemoryStream([1]), "ExtensionConversionNamedMixedTarget");

    [ModuleInitializer]
    public static void ExtensionConversionInit() =>
        VerifierSettings.RegisterStreamConverter(
            "ExtensionConversion",
            (_, _, _) => new(null, "txt", "Foo"));

    [Fact]
    public Task ExtensionConversion() =>
        Verify(new MemoryStream([1]), "ExtensionConversion");

    [ModuleInitializer]
    public static void AsyncExtensionConversionInit() =>
        VerifierSettings.RegisterStreamConverter(
            "AsyncExtensionConversion",
            (_, _, _) => Task.FromResult(new ConversionResult(null, "txt", "Foo")));

    [Fact]
    public Task AsyncExtensionConversion() =>
        Verify(new MemoryStream([1]), "AsyncExtensionConversion");

    [ModuleInitializer]
    public static void WithInfoInit() =>
        VerifierSettings.RegisterStreamConverter(
            "WithInfo",
            (_, _, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "txt", "Foo");
            });

    [Fact]
    public Task WithInfo() =>
        Verify(new MemoryStream([1]), "WithInfo");

    [Fact]
    public Task WithInfo_AndAppends() =>
        Verify(new MemoryStream([1]), "WithInfo")
            .AppendValue("AppendKey", "AppendValue");

    [Fact]
    public Task WithInfo_AndRecording()
    {
        Recording.Start();
        Recording.Add("RecordKey", "RecordValue");
        return Verify(new MemoryStream([1]), "WithInfo");
    }

    [ModuleInitializer]
    public static void WithInfoAndBinaryInit() =>
        VerifierSettings.RegisterStreamConverter(
            "WithInfoAndBinary",
            (_, stream, _) =>
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
                .DisableRequireUniquePrefix()
                .DisableDiff());
        var file = CurrentFile.Relative($"ExtensionConverterTests.WithInfoAndModifiedBinary.{Namer.RuntimeAndVersion}.received.png");
        Assert.True(File.Exists(file));
    }

#endif
}