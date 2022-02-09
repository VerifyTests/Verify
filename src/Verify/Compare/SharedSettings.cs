namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<string, StringCompare> stringComparers = new();
    static Dictionary<string, StreamCompare> streamComparers = new();
    static StringCompare? defaultStringComparer;

    internal static bool TryGetStreamComparer(string extension, [NotNullWhen(true)] out StreamCompare? comparer)
    {
        return streamComparers.TryGetValue(extension, out comparer);
    }

    internal static bool TryGetStringComparer(string extension, [NotNullWhen(true)] out StringCompare? comparer)
    {
        if (stringComparers.TryGetValue(extension, out comparer))
        {
            return true;
        }

        if (defaultStringComparer is not null)
        {
            comparer = defaultStringComparer;
            return true;
        }

        return false;
    }

    public static void RegisterStreamComparer(string extension, StreamCompare compare)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));
        streamComparers[extension] = compare;
    }

    public static void RegisterStringComparer(string extension, StringCompare compare)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));
        stringComparers[extension] = compare;
    }

    public static void SetDefaultStringComparer(StringCompare compare)
    {
        defaultStringComparer = compare;
    }
}