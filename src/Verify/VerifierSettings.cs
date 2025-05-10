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
        if (includeBuildServer ||
            !BuildServerDetector.Detected)
        {
            VerifierSettings.autoVerify = autoVerify;
            VerifierSettings.throwException = throwException;
        }
    }

    internal static GlobalAutoVerify? autoVerify;
    internal static bool throwException;

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