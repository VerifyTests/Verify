namespace Verify
{
    public partial class VerifySettings
    {
        internal FirstVerify? handleOnFirstVerify;

        public void OnFirstVerify(FirstVerify firstVerify)
        {
            Guard.AgainstNull(firstVerify, nameof(firstVerify));
            handleOnFirstVerify = firstVerify;
        }

        internal VerifyMismatch? handleOnVerifyMismatch;

        public void OnVerifyMismatch(VerifyMismatch verifyMismatch)
        {
            Guard.AgainstNull(verifyMismatch, nameof(verifyMismatch));
            handleOnVerifyMismatch = verifyMismatch;
        }
    }
}