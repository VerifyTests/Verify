using System.Diagnostics.CodeAnalysis;

namespace Verify
{
    public partial class VerifySettings
    {
        Compare? comparer;

        public void UseComparer(Compare compare)
        {
            Guard.AgainstNull(compare, nameof(compare));
            comparer = compare;
        }

        internal bool TryFindComparer([NotNullWhen(true)] out Compare? compare)
        {
            if (comparer != null)
            {
                compare = comparer;
                return true;
            }
            return SharedVerifySettings.TryGetComparer(ExtensionOrTxt(), out compare);
        }
    }
}