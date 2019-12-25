namespace Verify
{
    public partial class VerifySettings
    {
        internal bool clipboardEnabled = true;

        public void DisableClipboard()
        {
            clipboardEnabled = false;
        }
    }
}