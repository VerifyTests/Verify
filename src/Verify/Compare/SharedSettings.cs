using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static Dictionary<string, StringCompare> stringComparers = new();
        static Dictionary<string, StreamCompare> streamComparers = new();

        internal static bool TryGetStreamComparer(string extension, [NotNullWhen(true)] out StreamCompare? comparer)
        {
            return streamComparers.TryGetValue(extension, out comparer);
        }

        internal static bool TryGetStringComparer(string extension, [NotNullWhen(true)] out StringCompare? comparer)
        {
            return stringComparers.TryGetValue(extension, out comparer);
        }

        public static void RegisterStreamComparer(string extension, StreamCompare compare)
        {
            Guard.AgainstNull(compare, nameof(compare));
            Guard.AgainstBadExtension(extension, nameof(extension));
            streamComparers[extension] = compare;
        }

        public static void RegisterStringComparer(string extension, StringCompare compare)
        {
            Guard.AgainstNull(compare, nameof(compare));
            Guard.AgainstBadExtension(extension, nameof(extension));
            stringComparers[extension] = compare;
        }
    }
}