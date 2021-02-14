namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static FirstVerify? handleOnFirstVerify;

        public static void OnFirstVerify(FirstVerify firstVerify)
        {
            Guard.AgainstNull(firstVerify, nameof(firstVerify));
            handleOnFirstVerify = firstVerify;
        }

        internal static VerifyMismatch? handleOnVerifyMismatch;

        public static void OnVerifyMismatch(VerifyMismatch verifyMismatch)
        {
            Guard.AgainstNull(verifyMismatch, nameof(verifyMismatch));
            handleOnVerifyMismatch = verifyMismatch;
        }
    }
}