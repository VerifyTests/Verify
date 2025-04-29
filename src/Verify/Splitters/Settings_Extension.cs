namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<string, AsyncStreamConversion> extensionConverters = [];

    internal static bool TryGetStreamConverter(string extension, [NotNullWhen(true)] out AsyncStreamConversion? converter) =>
        extensionConverters.TryGetValue(extension, out converter);

    internal static bool HasStreamConverter(string extension) =>
        extensionConverters.ContainsKey(extension);

    [Obsolete("Use RegisterStreamConverter instead")]
    public static void RegisterFileConverter(
        string fromExtension,
        Conversion<Stream> conversion) =>
        RegisterFileConverter(
            fromExtension,
            (stream, context) => Task.FromResult(conversion(stream, context)));

    [Obsolete("Use RegisterStreamConverter instead")]
    public static void RegisterFileConverter(
        string fromExtension,
        AsyncConversion<Stream> conversion) =>
        RegisterStreamConverter(
            fromExtension,
            (_, stream, context) => conversion(stream, context));

    public static void RegisterStreamConverter(
        string fromExtension,
        StreamConversion conversion) =>
        RegisterStreamConverter(
            fromExtension,
            (name, stream, context) => Task.FromResult(conversion(name, stream, context)));

    public static void RegisterStreamConverter(
        string fromExtension,
        AsyncStreamConversion conversion)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guards.AgainstBadExtension(fromExtension);
        if (FileExtensions.IsTextExtension(fromExtension))
        {
            throw new("RegisterStreamConverter is only supported for non-text extensions");
        }

        extensionConverters[fromExtension] = conversion;
    }
}