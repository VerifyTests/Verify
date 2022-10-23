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

    /// <summary>
    /// Include <see cref="DateTimeOffset.Offset"/> in snapshot and parameter name.
    /// Requires <see cref="DontScrubDateTimes"/>.
    /// </summary>
    public static void IncludeDateOffset() =>
        includeDateOffset = true;
    internal static bool includeDateOffset;

    /// <summary>
    /// Include <see cref="DateTime.Kind"/> in snapshot and parameter name.
    /// Requires <see cref="DontScrubDateTimes"/>.
    /// </summary>
    public static void IncludeDateKind() =>
        includeDateKind = true;
    internal static bool includeDateKind;
}