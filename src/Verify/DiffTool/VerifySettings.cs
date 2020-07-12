using DiffEngine;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal bool diffEnabled = !DiffRunner.Disabled;

        public void DisableDiff()
        {
            diffEnabled = false;
        }
    }
}