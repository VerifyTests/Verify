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
}