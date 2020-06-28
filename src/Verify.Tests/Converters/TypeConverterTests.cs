﻿using System;
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
public class TypeConverterTests
{
    [Fact]
    public Task Inherited()
    {
        VerifierSettings.RegisterFileConverter<ParentClass>(
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("txt", x)));
            });

        var target = new InheritedClass
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
    public async Task WithStreamRequiringCleanup()
    {
        object? info = null;
        var filePath = "WithStreamRequiringCleanup.tmp";
        await File.WriteAllTextAsync(filePath, "FileContent");
        VerifierSettings.RegisterFileConverter<TargetForCleanup>(
            (instance, _) =>
            {
                #region ConversionResultWithCleanup
                return new ConversionResult(
                    info: info,
                    "txt",
                    stream: File.OpenRead(filePath),
                    cleanup: () =>
                    {
                        File.Delete(filePath);
                        return Task.CompletedTask;
                    });
                #endregion
            });
        var target = new TargetForCleanup
        {
            Value = "line1"
        };
        await Verifier.Verify(target);
        Assert.False(File.Exists(filePath));
    }

    public class TargetForCleanup
    {
        public string Value { get; set; } = null!;
    }

    [Fact]
    public Task ConvertWithNewline()
    {
        VerifierSettings.RegisterFileConverter<ClassToSplit>(
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("txt", x)));
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
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("txt", x)));
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
            (instance, _) =>
            {
                var streams = ToStream(instance.Value);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("txt", x)));
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
            (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, streams.Select(x => new ConversionStream("png", x)));
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
            canConvert: target => Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, streams.Select(x => new ConversionStream("png", x)));
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
            canConvert: target => Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("png", x)));
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