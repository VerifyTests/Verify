using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;
#if DEBUG
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

#endif

[UsesVerify]
public class TypeConverterTests
{
    [ModuleInitializer]
    public static void InheritedInit()
    {
        VerifierSettings.RegisterFileConverter<ParentClass>(
            (instance, _) => new(null, "txt", instance.Value));
    }

    [Fact]
    public Task Inherited()
    {
        InheritedClass target = new()
        {
            Value = "line1"
        };
        VerifySettings settings = new();
        settings.UseExtension("txt");
        return Verifier.Verify(target, settings);
    }

    class ParentClass
    {
        public string Value { get; set; } = null!;
    }

    class InheritedClass : ParentClass
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

    class TargetForCleanup
    {
        public string Value { get; set; } = null!;
    }

    [ModuleInitializer]
    public static void ConvertWithNewlineInit()
    {
        VerifierSettings.RegisterFileConverter<ClassToSplit>(
            (instance, _) => new(null, "txt", instance.Value));
    }

    [Fact]
    public Task ConvertWithNewline()
    {
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

    [ModuleInitializer]
    public static void ConvertWithCanConvert_InvalidInit()
    {
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            (instance, _) => new(null, "txt", instance.Value),
            (inner, _, _) => inner.Value == "Valid");
    }

    [Fact]
    public Task ConvertWithCanConvert_Invalid()
    {
        CanConvertTarget target = new()
        {
            Value = "Invalid"
        };
        return Verifier.Verify(target);
    }

    [ModuleInitializer]
    public static void ConvertWithCanConvert_ValidInit()
    {
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            (instance, _) => new(null, "txt", instance.Value),
            (inner, _, _) => inner.Value == "Valid");
    }

    [Fact]
    public Task ConvertWithCanConvert_Valid()
    {
        CanConvertTarget target = new()
        {
            Value = "Valid"
        };
        return Verifier.Verify(target);
    }

    class CanConvertTarget
    {
        public string Value { get; set; } = null!;
    }

#if DEBUG
    [ModuleInitializer]
    public static void WithInfoInit()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            (bitmap1, _) =>
            {
                var targets = ConvertBmpTpPngStreams(bitmap1);
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, targets.Select(x => new Target("png", x)));
            },
            (_, _, context) =>
            {
                return context.ContainsKey("name") &&
                       (string) context["name"] == nameof(WithInfo);
            });
    }

    [Fact]
    public Task WithInfo()
    {
        VerifySettings settings = new();
        settings.Context["name"] = nameof(WithInfo);
        Bitmap bitmap = new(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    [ModuleInitializer]
    public static void WithInfoShouldRespectSettingsInit()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            canConvert: (target, _, context) =>
                context.ContainsKey("name") &&
                (string) context["name"] == nameof(WithInfoShouldRespectSettings) &&
                Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var targets = ConvertBmpTpPngStreams(bitmap1);
                var info = new
                {
                    Property = "Value"
                };
                return new ConversionResult(info, targets.Select(x => new Target("png", x)));
            });
    }

    [Fact]
    public Task WithInfoShouldRespectSettings()
    {
        VerifySettings settings = new();
        settings.Context["name"] = nameof(WithInfoShouldRespectSettings);
        settings.ModifySerialization(_ => { _.IgnoreMember("Property"); });
        Bitmap bitmap = new(FileHelpers.OpenRead("sample.bmp"));
        return Verifier.Verify(bitmap, settings);
    }

    [ModuleInitializer]
    public static void TypeConversionInit()
    {
        VerifierSettings.RegisterFileConverter<Bitmap>(
            canConvert: (target, _, context) =>
                context.ContainsKey("name") &&
                (string) context["name"] == nameof(TypeConversion) &&
                Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap1, _) =>
            {
                var targets = ConvertBmpTpPngStreams(bitmap1);
                return new ConversionResult(null, targets.Select(x => new Target("png", x)));
            });
    }

    [Fact]
    public Task TypeConversion()
    {
        VerifySettings settings = new();
        settings.Context["name"] = nameof(TypeConversion);
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