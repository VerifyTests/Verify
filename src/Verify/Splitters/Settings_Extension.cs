namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<string, AsyncConversion<Stream>> extensionConverters = new();

    internal static bool TryGetExtensionConverter(string extension, [NotNullWhen(true)] out AsyncConversion<Stream>? converter) =>
        extensionConverters.TryGetValue(extension, out converter);
    internal static bool HasExtensionConverter(string extension) =>
        extensionConverters.ContainsKey(extension);

    public static void RegisterFileConverter(
        string fromExtension,
        Conversion<Stream> conversion) =>
        RegisterFileConverter(
            fromExtension,
            (stream, context) => Task.FromResult(conversion(stream, context)));

    public static void RegisterFileConverter(
        string fromExtension,
        AsyncConversion<Stream> conversion)
    {
        Guard.AgainstBadExtension(fromExtension, nameof(fromExtension));
        extensionConverters[fromExtension] = conversion;
    }
}