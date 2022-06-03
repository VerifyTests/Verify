using DiffEngine;

namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static bool omitContentFromException;

    public static void OmitContentFromException() =>
        omitContentFromException = true;

    /// <summary>
    /// Automatically accept the results of all tests.
    /// </summary>
    public static void AutoVerify(bool includeBuildServer = true)
    {
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
    }

    internal static bool autoVerify;
}