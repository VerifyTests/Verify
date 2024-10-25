namespace VerifyTests;

public static class VerifyCombinationSettings
{
    public static bool CaptureExceptionsEnabled { get; private set; }

    public static void CaptureExceptions() =>
        CaptureExceptionsEnabled = true;
}