namespace Verify
{
    public partial class VerifySettings
    {
        internal bool diffEnabled = ShouldEnableDiff();

        static bool ShouldEnableDiff()
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

        public void DisableDiff()
        {
            diffEnabled = false;
        }
    }
}