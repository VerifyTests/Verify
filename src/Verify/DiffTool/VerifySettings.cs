using DiffEngine;

namespace VerifyTests
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