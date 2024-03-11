namespace VerifyTests;

public delegate bool GlobalAutoVerify(string typeName, string methodName);

public static partial class VerifierSettings
{
    internal static bool omitContentFromException;

    public static void OmitContentFromException() =>
        omitContentFromException = true;

    /// <summary>
    /// Automatically accept the results of all tests.
    /// </summary>
    // ReSharper disable once UnusedParameter.Global
    public static void AutoVerify(bool includeBuildServer = true)
    {
    }

    /// <summary>
    /// Automatically accept the results of all tests.
    /// </summary>
    // ReSharper disable once UnusedParameter.Global
    public static void AutoVerify(GlobalAutoVerify autoVerify, bool includeBuildServer = true)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
#if DiffEngine
        if (includeBuildServer)
        {
            VerifierSettings.autoVerify = autoVerify;
        }
        else
        {
            if (!BuildServerDetector.Detected)
            {
                VerifierSettings.autoVerify = autoVerify;
            }
        }
#endif
    }

    internal static GlobalAutoVerify? autoVerify;

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