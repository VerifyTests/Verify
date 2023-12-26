namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<string, AsyncConversion<Stream>> extensionConverters = [];

    internal static bool TryGetExtensionConverter(string extension, [NotNullWhen(true)] out AsyncConversion<Stream>? converter) =>
        extensionConverters.TryGetValue(extension, out converter);

    internal static bool HasExtensionConverter(string extension) =>
        extensionConverters.ContainsKey(extension);

    public static void RegisterFileConverter(
        string fromExtension,
        Conversion<Stream> conversion)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        RegisterFileConverter(
            fromExtension,
            (stream, context) => Task.FromResult(conversion(stream, context)));
    }

    public static void RegisterFileConverter(
        string fromExtension,
        AsyncConversion<Stream> conversion)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guard.AgainstBadExtension(fromExtension);
        if (FileExtensions.IsTextExtension(fromExtension))
        {
            throw new("RegisterFileConverter is only supported for non-text extensions");
        }

        extensionConverters[fromExtension] = conversion;
    }
}