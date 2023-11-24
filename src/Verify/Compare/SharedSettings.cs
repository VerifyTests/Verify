namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<string, StringCompare> stringComparers = [];
    static Dictionary<string, StreamCompare> streamComparers = [];
    static StringCompare? defaultStringComparer;

    internal static bool TryGetStreamComparer(string extension, [NotNullWhen(true)] out StreamCompare? comparer) =>
        streamComparers.TryGetValue(extension, out comparer);

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
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guard.AgainstBadExtension(extension);
        streamComparers[extension] = compare;
    }

    public static void RegisterStringComparer(string extension, StringCompare compare)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guard.AgainstBadExtension(extension);
        stringComparers[extension] = compare;
    }

    public static void SetDefaultStringComparer(StringCompare compare)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        defaultStringComparer = compare;
    }
}