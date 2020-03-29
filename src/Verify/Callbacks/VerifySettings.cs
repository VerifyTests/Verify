namespace Verify
{
    public partial class VerifySettings
    {
        internal FirstVerify? handleOnFirstVerify;

        public void OnFirstVerify(FirstVerify func)
        {
            Guard.AgainstNull(func, nameof(func));
            handleOnFirstVerify = func;
        }

        internal VerifyMismatch? handleOnVerifyMismatch;

        public void OnVerifyMismatch(VerifyMismatch func)
        {
            Guard.AgainstNull(func, nameof(func));
            handleOnVerifyMismatch = func;
        }
    }
}