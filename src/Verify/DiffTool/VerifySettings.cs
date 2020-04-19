using DiffEngine;

namespace Verify
{
    public partial class VerifySettings
    {
        internal bool diffEnabled = !NCrunch.Enabled &&
                                    !BuildServerDetector.Detected;

        public void DisableDiff()
        {
            diffEnabled = false;
        }
    }
}