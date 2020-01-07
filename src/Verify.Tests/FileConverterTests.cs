#if(NETCOREAPP3_1)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class FileConverterTests :
    VerifyBase
{
    [Fact]
    public Task ConvertWithNewline()
    {
        SharedVerifySettings.RegisterFileConverter<ClassToSplit>("txt", ClassToStream);
        var target = new ClassToSplit
        {
            Value = $@"line1{Environment.NewLine}line2"
        };
        return Verify(target);
    }

    static IEnumerable<Stream> ClassToStream(ClassToSplit split)
    {
        var bytes = Encoding.UTF8.GetBytes(split.Value);
        yield return new MemoryStream(bytes);
    }

    public class ClassToSplit
    {
        public string Value { get; set; } = null!;
    }
    //TODO: a multiple split
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
#endif