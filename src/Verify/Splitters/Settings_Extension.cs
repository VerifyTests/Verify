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
        string extension,
        StreamConversion conversion) =>
        RegisterStreamConverter(
            extension,
            (name, stream, context) => Task.FromResult(conversion(name, stream, context)));

    public static void RegisterStreamConverter(
        string extension,
        AsyncStreamConversion conversion)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guards.AgainstBadExtension(extension);
        if (FileExtensions.IsTextExtension(extension))
        {
            throw new("RegisterStreamConverter is only supported for non-text extensions");
        }

        extensionConverters[extension] = conversion;
    }
}