namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static bool clipboardDisabled;

        public static void DisableClipboard()
        {
            clipboardDisabled = false;
        }
    }
}