namespace VerifyTesting
{
    public static partial class VerifierSettings
    {
        internal static bool clipboardDisabled;

        public static void DisableClipboard()
        {
            clipboardDisabled = false;
        }
    }
}