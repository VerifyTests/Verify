namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static bool clipboardDisabled;

        public static void DisableClipboard()
        {
            clipboardDisabled = true;
        }
    }
}