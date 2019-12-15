using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class FileConverterTests :
    VerifyBase
{
    [Fact]
    public Task ExtensionConversion()
    {
        SharedVerifySettings.RegisterFileConverter("bmp", "png", ConvertBmpTpPng);
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        return Verify(File.OpenRead("sample.bmp"), settings);
    }

    IEnumerable<Stream> ConvertBmpTpPng(Stream input)
    {
        var bitmap = new Bitmap(input);
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }

    [Fact]
    public Task TypeConversion()
    {
        SharedVerifySettings.RegisterFileConverter<Bitmap>("png", ConvertBmpTpPng);
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        var bitmap = new Bitmap(File.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
    }

    IEnumerable<Stream> ConvertBmpTpPng(Bitmap bitmap)
    {
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }

    public FileConverterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}