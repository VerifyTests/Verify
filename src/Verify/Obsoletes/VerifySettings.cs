namespace VerifyTests;

public partial class VerifySettings
{
    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [Obsolete("Use VerifySettings.AutoVerify(AutoVerify, autoVerify, bool includeBuildServer, bool throwException)")]
    public void AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true) =>
        AutoVerify(autoVerify, includeBuildServer, false);

    [Obsolete("Use VerifySettings.AutoVerify(bool includeBuildServer, bool throwException)")]
    public void AutoVerify(bool includeBuildServer = true) =>
        AutoVerify(includeBuildServer, false);

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void AutoVerify(bool includeBuildServer = true, bool throwException = false) =>
        AutoVerify(_ => true, includeBuildServer, throwException);

    [Obsolete("Use VerifySettings.UseStreamComparer(StreamCompare compare, params ReadOnlySpan<string> extensions)")]
    public void UseStreamComparer(StreamCompare compare) =>
        streamComparer = compare;

    [Obsolete("Use VerifySettings.UseStringComparer(StringCompare compare, params ReadOnlySpan<string> extensions)")]
    public void UseStringComparer(StringCompare compare) =>
        stringComparer = compare;

    [OverloadResolutionPriority(1)]
    public void UseStreamComparer(StreamCompare compare, params ReadOnlySpan<string> extensions)
    {
        if (extensions.Length == 0)
        {
            streamComparer = compare;
        }
        else
        {
            extensionStreamComparers ??= [];
            foreach (var extension in extensions)
            {
                Guards.AgainstBadExtension(extension);
                extensionStreamComparers[extension] = compare;
            }
        }
    }

    [OverloadResolutionPriority(1)]
    public void UseStringComparer(StringCompare compare, params ReadOnlySpan<string> extensions)
    {
        if (extensions.Length == 0)
        {
            stringComparer = compare;
        }
        else
        {
            extensionStringComparers ??= [];
            foreach (var extension in extensions)
            {
                Guards.AgainstBadExtension(extension);
                extensionStringComparers[extension] = compare;
            }
        }
    }
}