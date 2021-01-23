namespace VerifyTests
{
    public partial class VerifySettings
    {
        StreamCompare? streamComparer;
        StringCompare? stringComparer;

        public void UseStreamComparer(StreamCompare compare)
        {
            Guard.AgainstNull(compare, nameof(compare));
            streamComparer = compare;
        }

        public void UseStringComparer(StringCompare compare)
        {
            Guard.AgainstNull(compare, nameof(compare));
            stringComparer = compare;
        }

        internal bool TryFindStreamComparer(out StreamCompare? compare)
        {
            if (streamComparer != null)
            {
                compare = streamComparer;
                return true;
            }

            return VerifierSettings.TryGetStreamComparer(ExtensionOrBin(), out compare);
        }

        internal bool TryFindStringComparer(out StringCompare? compare)
        {
            if (stringComparer != null)
            {
                compare = stringComparer;
                return true;
            }

            return VerifierSettings.TryGetStringComparer(ExtensionOrTxt(), out compare);
        }
    }
}