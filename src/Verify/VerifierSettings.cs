namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static bool omitContentFromException;

    public static void OmitContentFromException() =>
        omitContentFromException = true;

    /// <summary>
    /// Automatically accept the results of all tests.
    /// </summary>
    // ReSharper disable once UnusedParameter.Global
    public static void AutoVerify(bool includeBuildServer = true, bool throwException = false) =>
        AutoVerify((_, _, _) => true, includeBuildServer, throwException);

    /// <summary>
    /// Automatically accept the results of all tests.
    /// </summary>
    // ReSharper disable once UnusedParameter.Global
    public static void AutoVerify(GlobalAutoVerify autoVerify, bool includeBuildServer = true, bool throwException = false)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (!includeBuildServer && BuildServerDetector.Detected)
        {
            return;
        }

        VerifierSettings.autoVerify = autoVerify;
        VerifierSettings.throwException = throwException;
    }

    internal static GlobalAutoVerify? autoVerify;
    internal static bool throwException;

    internal static bool fixNewlinesOnRead;

    /// <summary>
    /// Normalize `\r\n` and `\r` to `\n` when reading verified files, instead of rejecting a
    /// verified file that contains a carriage return.
    /// Has side effects. See https://github.com/VerifyTests/Verify/blob/main/docs/newline-tolerance.md
    /// </summary>
    public static void FixNewlinesOnRead()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        fixNewlinesOnRead = true;
    }

    internal static bool ignoreTrailingNewline;

    /// <summary>
    /// Treat a verified file that has a single trailing `\n`, where that newline is the only
    /// difference, as equal to the received content.
    /// Has side effects, and can mask a real change in trailing newlines. See
    /// https://github.com/VerifyTests/Verify/blob/main/docs/newline-tolerance.md
    /// </summary>
    public static void IgnoreTrailingNewline()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        ignoreTrailingNewline = true;
    }

    public static void UseUtf8NoBom() =>
        Encoding = new UTF8Encoding(false, true);

    public static void UseEncoding(Encoding encoding) =>
        Encoding = encoding;

    static Encoding encoding = new UTF8Encoding(true, true);

    internal static Encoding Encoding
    {
        get => encoding;
        private set
        {
            InnerVerifier.ThrowIfVerifyHasBeenRun();
            encoding = value;
        }
    }
}