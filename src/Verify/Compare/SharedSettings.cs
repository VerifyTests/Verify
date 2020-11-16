using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static Dictionary<string, Compare> comparers = new();

        internal static bool TryGetComparer(string extension, [NotNullWhen(true)] out Compare? comparer)
        {
            return comparers.TryGetValue(extension, out comparer);
        }

        public static void RegisterComparer(string extension, Compare compare)
        {
            Guard.AgainstNull(compare, nameof(compare));
            Guard.AgainstBadExtension(extension, nameof(extension));
            comparers[extension] = compare;
        }
    }
}