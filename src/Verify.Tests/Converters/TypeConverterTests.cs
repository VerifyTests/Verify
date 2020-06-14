#if(NETCOREAPP3_1)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TypeConverterTests
{
    [Fact]
    public Task Inherited()
    {
        VerifierSettings.RegisterFileConverter<ParentClass>(
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
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task DifferingExtensions()
    {
        VerifierSettings.RegisterFileConverter<ClassToSplit3>(
            "notTxt",
            async (classToSplit, _) =>
            {
                await Task.Delay(1);
                throw new Exception();
            });

        VerifierSettings.RegisterFileConverter<ClassToSplit3>(
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
        return Verifier.Verify(target, settings);
    }

    static List<Stream> ToStream(string splitValue)
    {
        var bytes = FileHelpers.Utf8NoBOM.GetBytes(splitValue);
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
        VerifierSettings.RegisterFileConverter<ClassToSplit2>(
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
        return Verifier.Verify(target, settings);
    }

    public class ClassToSplit2
    {
        public string Value { get; set; } = null!;
    }

    [Fact]
    public Task ConvertWithNewline()
    {
        VerifierSettings.RegisterFileConverter<ClassToSplit>(
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
        return Verifier.Verify(target);
    }

    public class ClassToSplit
    {
        public string Value { get; set; } = null!;
    }

    [Fact]
    public Task ConvertWithCanConvert_Invalid()
    {
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            "txt",
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams);
            },
            o => o.Value == "Valid");
        var target = new CanConvertTarget
        {
            Value = "Invalid"
        };
        return Verifier.Verify(target);
    }

    [Fact]
    public Task ConvertWithCanConvert_Valid()
    {
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            "txt",
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams);
            },
            o => o.Value == "Valid");
        var target = new CanConvertTarget
        {
            Value = "Valid"
        };
        return Verifier.Verify(target);
    }

    public class CanConvertTarget
    {
        public string Value { get; set; } = null!;
    }

    [Fact]
    public Task WithInfo()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
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
        return Verifier.Verify(bitmap, settings);
    }

    [Fact]
    public Task WithInfoShouldRespectSettings()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            "png",
            canConvert: target => Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
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
        settings.ModifySerialization(_ => { _.IgnoreMember("Property"); });
        var bitmap = new Bitmap(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    [Fact]
    public Task TypeConversion()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            "png",
            canConvert: target => Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                return new ConversionResult(null, streams);
            });
        var settings = new VerifySettings();
        settings.UseExtension("bmp");
        var bitmap = new Bitmap(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    static IEnumerable<Stream> ConvertBmpTpPngStreams(Bitmap bitmap)
    {
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }
}
#endif