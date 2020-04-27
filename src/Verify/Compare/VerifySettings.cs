namespace Verify
{
    public partial class VerifySettings
    {
        internal Compare? comparer;

        public void UseComparer(Compare func)
        {
            Guard.AgainstNull(func, nameof(func));
            comparer = func;
        }
    }
}