﻿#pragma warning disable CA1416
#if NET48 && DEBUG
public class TypeConverterTests
{
    [ModuleInitializer]
    public static void InheritedInit() =>
        VerifierSettings.RegisterFileConverter<ParentClass>(
            (instance, _) => new(null, "txt", instance.Value));

    [Fact]
    public Task Inherited()
    {
        var target = new InheritedClass
        {
            Value = "line1"
        };
        return Verify(target);
    }

    class ParentClass
    {
        public string Value { get; set; } = null!;
    }

    class InheritedClass : ParentClass;

    [ModuleInitializer]
    public static void WithStreamRequiringCleanupInit() =>
        VerifierSettings.RegisterFileConverter<TargetForCleanup>(
            (_, _) =>
            {
                #region ConversionResultWithCleanup

                return new(
                    info: null,
                    "bin",
                    stream: File.OpenRead(withStreamRequiringCleanupPath),
                    cleanup: () =>
                    {
                        File.Delete(withStreamRequiringCleanupPath);
                        return Task.CompletedTask;
                    });

                #endregion
            });

    static string withStreamRequiringCleanupPath = "WithStreamRequiringCleanup.tmp";

    [Fact]
    public async Task WithStreamRequiringCleanup()
    {
        File.WriteAllText(withStreamRequiringCleanupPath, "FileContent");
        var target = new TargetForCleanup("line1");
        await Verify(target);
        Assert.False(File.Exists(withStreamRequiringCleanupPath));
    }

    public record TargetForCleanup(string Value);

    [ModuleInitializer]
    public static void ConvertWithNewlineInit() =>
        VerifierSettings.RegisterFileConverter<ClassToSplit>(
            (instance, _) => new(null, "txt", instance.Value));

    [Fact]
    public Task ConvertWithNewline()
    {
        var target = new ClassToSplit($"line1{Environment.NewLine}line2");
        return Verify(target);
    }

    public record ClassToSplit(string Value);

    [ModuleInitializer]
    public static void ConvertWithCanConvert_InvalidInit() =>
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            (instance, _) => new(null, "txt", instance.Value),
            (inner, _) => inner.Value == "Valid");

    [Fact]
    public Task ConvertWithCanConvert_Invalid()
    {
        var target = new CanConvertTarget("Invalid");
        return Verify(target);
    }

    [ModuleInitializer]
    public static void ConvertWithCanConvert_ValidInit() =>
        VerifierSettings.RegisterFileConverter<CanConvertTarget>(
            (instance, _) => new(null, "txt", instance.Value),
            (inner, _) => inner.Value == "Valid");

    [Fact]
    public Task ConvertWithCanConvert_Valid()
    {
        var target = new CanConvertTarget("Valid");
        return Verify(target);
    }

    record CanConvertTarget(string Value);

    [ModuleInitializer]
    public static void WithInfoInit() =>
        VerifierSettings.RegisterFileConverter<Bitmap>(
            (bitmap, _) =>
            {
                var targets = ConvertBmpTpPngStreams(bitmap);
                var info = new
                {
                    Property = "Value"
                };
                return new(info, targets.Select(_ => new Target("png", _)));
            },
            (_, context) => context.ContainsKey("name") &&
                            (string) context["name"] == nameof(WithInfo));

    [Fact(Skip = "flakey")]
    public Task WithInfo()
    {
        var settings = new VerifySettings
        {
            Context =
            {
                ["name"] = nameof(WithInfo)
            }
        };
        var bitmap = new Bitmap(IoHelpers.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
    }

    [ModuleInitializer]
    public static void WithInfoShouldRespectSettingsInit() =>
        VerifierSettings.RegisterFileConverter<Bitmap>(
            canConvert: (target, context) =>
                context.ContainsKey("name") &&
                (string) context["name"] == nameof(WithInfoShouldRespectSettings) &&
                Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap, _) =>
            {
                var targets = ConvertBmpTpPngStreams(bitmap);
                var info = new
                {
                    Property = "Value"
                };
                return new(info, targets.Select(_ => new Target("png", _)));
            });

    [Fact(Skip = "flakey")]
    public Task WithInfoShouldRespectSettings()
    {
        var settings = new VerifySettings
        {
            Context =
            {
                ["name"] = nameof(WithInfoShouldRespectSettings)
            }
        };
        settings.IgnoreMember("Property");
        var bitmap = new Bitmap(IoHelpers.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
    }

    [ModuleInitializer]
    public static void TypeConversionInit() =>
        VerifierSettings.RegisterFileConverter<Bitmap>(
            canConvert: (target, context) =>
                context.ContainsKey("name") &&
                (string) context["name"] == nameof(TypeConversion) &&
                Equals(target.RawFormat, ImageFormat.Bmp),
            conversion: (bitmap, _) =>
            {
                var streams = ConvertBmpTpPngStreams(bitmap);
                return new ConversionResult(null, streams.Select(_ => new Target("png", _)));
            });

    [Fact(Skip = "flakey")]
    public Task TypeConversion()
    {
        var settings = new VerifySettings
        {
            Context =
            {
                ["name"] = nameof(TypeConversion)
            }
        };
        var bitmap = new Bitmap(IoHelpers.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
    }

    static IEnumerable<Stream> ConvertBmpTpPngStreams(Bitmap bitmap)
    {
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }
}
#endif