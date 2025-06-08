namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static bool addAttachments = true;

    /// <summary>
    /// Disables adding received files as test attachments.
    /// </summary>
    public static void DisableAttachments() => addAttachments = false;
}