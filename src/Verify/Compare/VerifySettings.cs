namespace VerifyTests;

public partial class VerifySettings
{
    StreamCompare? streamComparer;
    StringCompare? stringComparer;
    Dictionary<string, StreamCompare>? extensionStreamComparers;
    Dictionary<string, StringCompare>? extensionStringComparers;

    // Dont use this.extension since a converter may have
    // changed the extension for the current compare operation
    internal bool TryFindStreamComparer(string extension, [NotNullWhen(true)] out StreamCompare? compare)
    {
        if (extensionStreamComparers != null &&
            extensionStreamComparers.TryGetValue(extension, out compare))
        {
            return true;
        }

        if (streamComparer is not null)
        {
            compare = streamComparer;
            return true;
        }

        return VerifierSettings.TryGetStreamComparer(extension, out compare);
    }

    internal bool TryFindStringComparer(string extension, [NotNullWhen(true)] out StringCompare? compare)
    {
        if (extensionStringComparers != null &&
            extensionStringComparers.TryGetValue(extension, out compare))
        {
            return true;
        }

        if (stringComparer is not null)
        {
            compare = stringComparer;
            return true;
        }

        return VerifierSettings.TryGetStringComparer(extension, out compare);
    }
}