#if(NETCOREAPP3_1 && DEBUG)
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            "txt",
            (stream, _) => new ConversionResult(null, "txt", stream));
        var settings = new VerifySettings();
        settings.UseExtension("txt");
        return Verifier.Verify(FileHelpers.OpenRead("sample.split"), settings);
    }

    [Fact]
    public Task ExtensionConversion()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            "png",
            (stream, _) =>
            {
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(null, "png", streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"), settings);
    }

    [Fact]
    public Task AsyncExtensionConversion()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            "png",
            (stream, _) =>
            {
                var streams = ConvertBmpTpPngStreams(stream);
                return Task.FromResult(new ConversionResult(null, "png", streams));
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"), settings);
    }

    [Fact]
    public Task WithInfo()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            "png",
            (stream, _) =>
            {
                var info = new
                {
                    Property ="Value"
                };
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(info, "png", streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"), settings);
    }

    static IEnumerable<Stream> ConvertBmpTpPngStreams(Stream input)
    {
        var bitmap = new Bitmap(input);
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }
}
#endif