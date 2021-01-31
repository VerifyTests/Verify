using DiffEngine;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal bool diffEnabled = !DiffRunner.Disabled;

        /// <summary>
        /// Disable using a diff toll for this test
        /// </summary>
        public void DisableDiff()
        {
            diffEnabled = false;
        }
    }
}