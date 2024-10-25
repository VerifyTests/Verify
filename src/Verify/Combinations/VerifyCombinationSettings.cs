namespace VerifyTests;

public static class VerifyCombinationSettings
{
    internal static bool captureExceptions;

    public static void CaptureExceptions() =>
        captureExceptions = true;
}