using DiffEngine;

namespace Verify
{
    public partial class VerifySettings
    {
        internal bool clipboardEnabled = !NCrunch.Enabled &&
                                         !BuildServerDetector.Detected;

        public void DisableClipboard()
        {
            clipboardEnabled = false;
        }
    }
}