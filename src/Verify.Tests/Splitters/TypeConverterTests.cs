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

public class TypeConverterTests :
    VerifyBase
{
    [Fact]
    public Task Inherited()
    {
        SharedVerifySettings.RegisterFileConverter<ParentClass>(
            "txt",
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams);
            });

        var target = new InheritedClass
        {
            Value = "line1"
        };
        var settings = new VerifySettings();
        settings.UseExtension("txt");
        return Verify(target, settings);
    }

    [Fact]
    public Task DifferingExtensions()
    {
        SharedVerifySettings.RegisterFileConverter<ClassToSplit3>(
            "notTxt", async (classToSplit, _) =>
            {
                await Task.Delay(1);
                throw new Exception();
            });

        SharedVerifySettings.RegisterFileConverter<ClassToSplit3>(
            "txt",
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams);
            });

        var target = new ClassToSplit3
        {
            Value = "line1"
        };
        var settings = new VerifySettings();
        settings.UseExtension("txt");
        return Verify(target, settings);
    }

    static List<Stream> ToStream(string splitValue)
    {
        var bytes = Encoding.UTF8.GetBytes(splitValue);
        return new List<Stream> {new MemoryStream(bytes)};
    }

    public class ClassToSplit3
    {
        public string Value { get; set; } = null!;
    }

    public class ParentClass
    {
        public string Value { get; set; } = null!;
    }

    public class InheritedClass : ParentClass
    {
    }

    [Fact]
    public Task ConvertWithExtInSettings()
    {
        SharedVerifySettings.RegisterFileConverter<ClassToSplit2>(
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams);
            });
        var target = new ClassToSplit2
        {
            Value = "line1"
        };
        var settings = new VerifySettings();
        settings.UseExtension("txt");
        return Verify(target, settings);
    }

    public class ClassToSplit2
    {
        public string Value { get; set; } = null!;
    }

    [Fact]
    public Task ConvertWithNewline()
    {
        SharedVerifySettings.RegisterFileConverter<ClassToSplit>(
            "txt",
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams);
            });
        var target = new ClassToSplit
        {
            Value = $@"line1{Environment.NewLine}line2"
        };
        return Verify(target);
    }

    public class ClassToSplit
    {
        public string Value { get; set; } = null!;
    }

    [Fact]
    public Task WithInfo()
    {
        SharedVerifySettings.RegisterFileConverter<Bitmap>(
            "png",
            (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        var bitmap = new Bitmap(FileHelpers.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
    }

    [Fact]
    public Task TypeConversion()
    {
        SharedVerifySettings.RegisterFileConverter<Bitmap>(
            "png",
            (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                return new ConversionResult(null, streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        var bitmap = new Bitmap(FileHelpers.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
    }

    IEnumerable<Stream> ConvertBmpTpPngStreams(Bitmap bitmap)
    {
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }

    public TypeConverterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endif