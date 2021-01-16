using System;
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
            (instance, _) => new(null, "txt", instance.Value));

        InheritedClass target = new()
        {
            Value = "line1"
        };
        VerifySettings settings = new();
        settings.UseExtension("txt");
        return Verifier.Verify(target, settings);
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
        File.WriteAllText(filePath, "FileContent");
        VerifierSettings.RegisterFileConverter<TargetForCleanup>(
            (_, _) =>
            {
                #region ConversionResultWithCleanup

                return new ConversionResult(
                    info: info,
                    "bin",
                    stream: File.OpenRead(filePath),
                    cleanup: () =>
                    {
                        File.Delete(filePath);
                        return Task.CompletedTask;
                    });

                #endregion
            });
        TargetForCleanup target = new()
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
            (instance, _) => new(null, "txt", instance.Value));
        ClassToSplit target = new()
        {
            Value = $"line1{Environment.NewLine}line2"
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
            (instance, _) => new(null, "txt", instance.Value),
            (inner, _, _) => inner.Value == "Valid");
        CanConvertTarget target = new()
        {
            Value = "Invalid"
        };
        return Verifier.Verify(target);
    }

    [Fact]
    public Task ConvertWithCanConvert_Valid()
    {
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            (instance, _) => new(null, "txt", instance.Value),
            (inner, _, _) => inner.Value == "Valid");
        CanConvertTarget target = new()
        {
            Value = "Valid"
        };
        return Verifier.Verify(target);
    }

    public class CanConvertTarget
    {
        public string Value { get; set; } = null!;
    }

#if DEBUG
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
        VerifySettings settings = new();
        settings.UseExtension("bmp");
        Bitmap bitmap = new(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    [Fact]
    public Task WithInfoShouldRespectSettings()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            canConvert: (target, _, _) => Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, streams.Select(x => new ConversionStream("png", x)));
            });
        VerifySettings settings = new();
        settings.UseExtension("bmp");
        settings.ModifySerialization(_ => { _.IgnoreMember("Property"); });
        Bitmap bitmap = new(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    [Fact]
    public Task TypeConversion()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            canConvert: (target, _, _) => Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap1);
                return new ConversionResult(null, streams.Select(x => new ConversionStream("png", x)));
            });
        VerifySettings settings = new();
        settings.UseExtension("bmp");
        Bitmap bitmap = new(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    static IEnumerable<Stream> ConvertBmpTpPngStreams(Bitmap bitmap)
    {
        MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }
#endif
}