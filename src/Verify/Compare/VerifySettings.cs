namespace VerifyTests;

public partial class VerifySettings
{
    StreamCompare? streamComparer;
    StringCompare? stringComparer;

    public void UseStreamComparer(StreamCompare compare)
    {
        streamComparer = compare;
    }

    public void UseStringComparer(StringCompare compare)
    {
        stringComparer = compare;
    }

    // Dont use this.extension since a converter may have
    // changed the extension for the current compare operation
    internal bool TryFindStreamComparer(string extension, [NotNullWhen(true)] out StreamCompare? compare)
    {
        if (streamComparer is not null)
        {
            compare = streamComparer;
            return true;
        }

        return VerifierSettings.TryGetStreamComparer(extension, out compare);
    }

    internal bool TryFindStringComparer(string extension, [NotNullWhen(true)] out StringCompare? compare)
    {
        if (stringComparer is not null)
        {
            compare = stringComparer;
            return true;
        }

        return VerifierSettings.TryGetStringComparer(extension, out compare);
    }
}