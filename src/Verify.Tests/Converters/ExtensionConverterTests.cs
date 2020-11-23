#if DEBUG
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            (stream, _) => new ConversionResult(null, "txt", stream));
        return Verifier.Verify(FileHelpers.OpenRead("sample.split"))
            .UseExtension("txt");
    }

    [Fact]
    public Task ExtensionConversion()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            (stream, _) =>
            {
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("png", x)));
            });
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"))
            .UseExtension("bmp");
    }

    [Fact]
    public Task AsyncExtensionConversion()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            (stream, _) =>
            {
                var streams = ConvertBmpTpPngStreams(stream);
                return Task.FromResult(new ConversionResult(null, streams.Select(x => new ConversionStream("png", x))));
            });
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"))
            .UseExtension("bmp");
    }

    [Fact]
    public Task WithInfo()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            (stream, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(info, streams.Select(x => new ConversionStream("png", x)));
            });
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"))
            .UseExtension("bmp");
    }

    static IEnumerable<Stream> ConvertBmpTpPngStreams(Stream input)
    {
        Bitmap bitmap = new(input);
        MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }
}
#endif