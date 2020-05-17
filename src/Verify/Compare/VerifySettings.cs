using System.Diagnostics.CodeAnalysis;

namespace Verify
{
    public partial class VerifySettings
    {
        Compare? comparer;

        public void UseComparer(Compare func)
        {
            Guard.AgainstNull(func, nameof(func));
            comparer = func;
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