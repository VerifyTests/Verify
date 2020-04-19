namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static bool? clipboardEnabled;

        public static void DisableClipboard()
        {
            clipboardEnabled = false;
        }

        public static void EnableClipboard()
        {
            clipboardEnabled = true;
        }
    }
}