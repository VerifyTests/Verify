#if(NETCOREAPP3_1)
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ExtensionConverterTests :
    VerifyBase
{
    [Fact]
    public Task ExtensionConversion()
    {
        SharedVerifySettings.RegisterFileConverter(
            "bmp",
            "png",
            (stream, _) =>
            {
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(null, streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        return Verify(File.OpenRead("sample.bmp"), settings);
    }
    [Fact]
    public Task WithInfo()
    {
        SharedVerifySettings.RegisterFileConverter(
            "bmp",
            "png",
            (stream, _) =>
            {
                var info = new
                {
                    Property ="Value"
                };
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(info, streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        return Verify(File.OpenRead("sample.bmp"), settings);
    }

    IEnumerable<Stream> ConvertBmpTpPngStreams(Stream input)
    {
        var bitmap = new Bitmap(input);
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }

    public ExtensionConverterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endif