namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<TypeConverter> typedConverters = null!;

    static void InitBuiltInTypedConverters() =>
        typedConverters =
        [
            // XContainer (XDocument, XElement) - scrub and return as xml
            new(
                (target, settings) => Task.FromResult(new ConversionResult(null, "xml", XmlScrubber.Scrub((XContainer) target, settings))),
                (target, _) => target is XContainer),

            // XmlNode - convert to XDocument, scrub, and return as xml
            new(
                (target, settings) =>
                {
                    using var reader = new XmlNodeReader((XmlNode) target);
                    reader.MoveToContent();
                    var xdoc = XDocument.Load(reader);
                    return Task.FromResult(new ConversionResult(null, "xml", XmlScrubber.Scrub(xdoc, settings)));
                },
                (target, _) => target is XmlNode),

            // StringBuilder - use "txt" extension
            new(
                (target, _) => Task.FromResult(new ConversionResult(null, "txt", (StringBuilder) target)),
                (target, _) => target is StringBuilder)
        ];

    internal static bool TryGetTypedConverter<T>(
        T target,
        VerifySettings settings,
        [NotNullWhen(true)] out TypeConverter? converter)
        where T : notnull
    {
        foreach (var typedConverter in typedConverters
                     .Where(_ => _.CanConvert(target, settings.Context)))
        {
            converter = typedConverter;
            return true;
        }

        converter = null;
        return false;
    }

    public static void RegisterFileConverter<T>(
        Conversion<T> conversion,
        CanConvert<T>? canConvert = null)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        RegisterFileConverter(
            (target, settings) => Task.FromResult(conversion(target, settings)),
            canConvert);
    }

    public static void RegisterFileConverter<T>(
        AsyncConversion<T> conversion,
        CanConvert<T>? canConvert = null)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var converter = new TypeConverter((target, settings) => conversion((T) target, settings), DefaultCanConvert(canConvert));
        // Insert at beginning so user converters take precedence over built-in converters
        typedConverters.Insert(0, converter);
    }

    public static void RegisterFileConverter(
        Conversion conversion,
        CanConvert canConvert)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        RegisterFileConverter(
            (target, settings) => Task.FromResult(conversion(target, settings)),
            canConvert);
    }

    public static void RegisterFileConverter(
        AsyncConversion conversion,
        CanConvert canConvert)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var converter = new TypeConverter(conversion, canConvert);
        // Insert at beginning so user converters take precedence over built-in converters
        typedConverters.Insert(0, converter);
    }

    static CanConvert DefaultCanConvert<T>(CanConvert<T>? canConvert)
    {
        if (canConvert is null)
        {
            return (target, _) => target is T;
        }

        return (target, settings) =>
        {
            if (target is T cast)
            {
                return canConvert(cast, settings);
            }

            return false;
        };
    }
}