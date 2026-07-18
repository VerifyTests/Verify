namespace VerifyTests;

public partial class VerifySettings
{
    [Obsolete("Use VerifySettings.UseStreamComparer(StreamCompare compare, params ReadOnlySpan<string> extensions)")]
    public void UseStreamComparer(StreamCompare compare) =>
        streamComparer = compare;

    [Obsolete("Use VerifySettings.UseStringComparer(StringCompare compare, params ReadOnlySpan<string> extensions)")]
    public void UseStringComparer(StringCompare compare) =>
        stringComparer = compare;
}
