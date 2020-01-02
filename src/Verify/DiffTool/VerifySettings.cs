namespace Verify
{
    public partial class VerifySettings
    {
        internal bool diffEnabled = true;

        public void DisableDiff()
        {
            diffEnabled = false;
        }
    }
}