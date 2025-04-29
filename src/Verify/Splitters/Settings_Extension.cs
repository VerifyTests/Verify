#pragma warning disable RegisterStreamScrubber
namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<string, AsyncConversion<Stream>> extensionConverters = [];
    static Dictionary<string, StreamScrubber> streamScrubbers = [];

    internal static bool TryGetExtensionConverter(string extension, [NotNullWhen(true)] out AsyncConversion<Stream>? converter) =>
        extensionConverters.TryGetValue(extension, out converter);

    internal static bool HasExtensionConverter(string extension) =>
        extensionConverters.ContainsKey(extension);

    internal static bool TryGetStreamScrubber(string extension, [NotNullWhen(true)] out StreamScrubber? converter) =>
        streamScrubbers.TryGetValue(extension, out converter);

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
        Guards.AgainstBadExtension(fromExtension);
        if (FileExtensions.IsTextExtension(fromExtension))
        {
            throw new("RegisterFileConverter is only supported for non-text extensions");
        }

        extensionConverters[fromExtension] = conversion;
    }

    [Experimental("RegisterStreamScrubber")]
    public static void RegisterStreamScrubber(
        string extension,
        StreamScrubber conversion)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guards.AgainstBadExtension(extension);
        if (FileExtensions.IsTextExtension(extension))
        {
            throw new("RegisterStreamScrubber is only supported for non-text extensions");
        }

        streamScrubbers[extension] = conversion;
    }
}