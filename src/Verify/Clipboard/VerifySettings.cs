namespace Verify
{
    public partial class VerifySettings
    {
        internal bool clipboardEnabled = ShouldEnableClipboard();

        static bool ShouldEnableClipboard()
        {
            if (NCrunch.Enabled())
            {
                return false;
            }

            if (BuildServerDetector.Detected)
            {
                return false;
            }
            return true;
        }

        public void DisableClipboard()
        {
            clipboardEnabled = false;
        }
    }
}