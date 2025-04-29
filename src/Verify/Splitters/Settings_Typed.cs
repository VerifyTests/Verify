namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<TypeConverter> typedConverters = [];

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
            (target, context) => Task.FromResult(conversion(target, context)),
            canConvert);
    }

    public static void RegisterFileConverter<T>(
        AsyncConversion<T> conversion,
        CanConvert<T>? canConvert = null)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var converter = new TypeConverter((target, context) => conversion((T) target, context), DefaultCanConvert(canConvert));
        typedConverters.Add(converter);
    }

    public static void RegisterFileConverter(
        Conversion conversion,
        CanConvert canConvert)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        RegisterFileConverter(
            (target, context) => Task.FromResult(conversion(target, context)),
            canConvert);
    }

    public static void RegisterFileConverter(
        AsyncConversion conversion,
        CanConvert canConvert)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var converter = new TypeConverter(conversion, canConvert);
        typedConverters.Add(converter);
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