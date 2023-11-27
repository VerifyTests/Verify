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
    public static void AutoVerify(bool includeBuildServer = true)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
#if DiffEngine
        if (includeBuildServer)
        {
            autoVerify = true;
        }
        else
        {
            if (!BuildServerDetector.Detected)
            {
                autoVerify = true;
            }
        }
#endif
    }

    internal static bool autoVerify;

    public static void UseUtf8NoBom() =>
        GlobalEncoding = new UTF8Encoding(false, true);

    public static void UseEncoding(Encoding encoding) =>
        GlobalEncoding = encoding;

    static Encoding globalEncoding = new UTF8Encoding(true, true);

    internal static Encoding GlobalEncoding
    {
        get => globalEncoding;
        private set
        {
            InnerVerifier.ThrowIfVerifyHasBeenRun();
            globalEncoding = value;
        }
    }
}