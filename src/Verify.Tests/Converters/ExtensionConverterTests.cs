using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ExtensionConverterTests
{
    [Fact]
    public Task TextSplit()
    {
        VerifierSettings.RegisterFileConverter(
            "split",
            (stream, _) => new(null, "txt", stream));
        return Verifier.Verify(FileHelpers.OpenRead("sample.split"))
            .UseExtension("txt");
    }

    [Fact]
    public Task ExtensionConversion()
    {
        VerifierSettings.RegisterFileConverter(
            "ExtensionConversion",
            (_, _) =>
            {
                return new ConversionResult(null, new[] {new Target("txt", "Foo")});
            });
        return Verifier.Verify(new MemoryStream())
            .UseExtension("ExtensionConversion");
    }

    [Fact]
    public Task AsyncExtensionConversion()
    {
        VerifierSettings.RegisterFileConverter(
            "AsyncExtensionConversion",
            (_, _) =>
            {
                return Task.FromResult(new ConversionResult(null,  new []{new Target("txt", "Foo")}));
            });
        return Verifier.Verify(new MemoryStream())
            .UseExtension("AsyncExtensionConversion");
    }

    [Fact]
    public Task WithInfo()
    {
        VerifierSettings.RegisterFileConverter(
            "WithInfo",
            (_, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, new []{new Target("txt", "Foo")});
            });
        return Verifier.Verify(new MemoryStream())
            .UseExtension("WithInfo");
    }
}