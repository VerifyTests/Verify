namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal bool? clipboardEnabled;

        public void DisableClipboard()
        {
            clipboardEnabled = false;
        }

        public void EnableClipboard()
        {
            clipboardEnabled = true;
        }
    }
}