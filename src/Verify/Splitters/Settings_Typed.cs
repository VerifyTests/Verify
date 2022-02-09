namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<TypeConverter> typedConverters = new();

    internal static bool TryGetTypedConverter<T>(
        T target,
        VerifySettings settings,
        [NotNullWhen(true)] out TypeConverter? converter)
    {
        foreach (var typedConverter in typedConverters
            .Where(_ => _.CanConvert(target!, settings.extension, settings.Context)))
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
        RegisterFileConverter(
            (o, context) => Task.FromResult(conversion(o, context)),
            canConvert);
    }

    public static void RegisterFileConverter<T>(
        AsyncConversion<T> conversion,
        CanConvert<T>? canConvert = null)
    {
        var converter = new TypeConverter((o, context) => conversion((T) o, context), DefaultCanConvert(canConvert));
        typedConverters.Add(converter);
    }

    public static void RegisterFileConverter(
        Conversion conversion,
        CanConvert canConvert)
    {
        RegisterFileConverter(
            (o, context) => Task.FromResult(conversion(o, context)),
            canConvert);
    }

    public static void RegisterFileConverter(
        AsyncConversion conversion,
        CanConvert canConvert)
    {
        var converter = new TypeConverter(conversion, canConvert);
        typedConverters.Add(converter);
    }

    static CanConvert DefaultCanConvert<T>(CanConvert<T>? canConvert)
    {
        if (canConvert is null)
        {
            return (target,_ , _) =>
            {
                return target is T;
            };
        }

        return (target, extension, settings) =>
        {
            if (target is T cast)
            {
                return canConvert(cast, extension, settings);
            }

            return false;
        };
    }
}