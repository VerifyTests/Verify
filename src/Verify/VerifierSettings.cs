namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static bool omitContentFromException;

    public static void OmitContentFromException()
    {
        omitContentFromException = true;
    }
}