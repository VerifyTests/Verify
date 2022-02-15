#if NET6_0 && DEBUG
using System.Drawing;
using System.Drawing.Imaging;

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
        var target = new InheritedClass
        {
            Value = "line1"
        };
        var settings = new VerifySettings();
        settings.UseExtension("txt");
        return Verify(target, settings);
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

                return new(
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
        var target = new TargetForCleanup
        {
            Value = "line1"
        };
        await Verify(target);
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
        var target = new ClassToSplit
        {
            Value = $"line1{Environment.NewLine}line2"
        };
        return Verify(target);
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
        var target = new CanConvertTarget
        {
            Value = "Invalid"
        };
        return Verify(target);
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
        var target = new CanConvertTarget
        {
            Value = "Valid"
        };
        return Verify(target);
    }

    class CanConvertTarget
    {
        public string Value { get; set; } = null!;
    }

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
                return new(info, targets.Select(x => new Target("png", x)));
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
                return new(info, targets.Select(x => new Target("png", x)));
            });
    }

    [Fact]
    public Task WithInfoShouldRespectSettings()
    {
        var settings = new VerifySettings
        {
            Context =
            {
                ["name"] = nameof(WithInfoShouldRespectSettings)
            }
        };
        settings.ModifySerialization(_ => _.IgnoreMember("Property"));
        var bitmap = new Bitmap(IoHelpers.OpenRead("sample.bmp"));
        return Verify(bitmap, settings);
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