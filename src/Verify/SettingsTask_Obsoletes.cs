namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.UseStreamComparer(StreamCompare)"/>
    [Pure]
    [Obsolete("Use UseStreamComparer(StreamCompare compare, params ReadOnlySpan<string> extensions)")]
    public SettingsTask UseStreamComparer(StreamCompare compare)
    {
        CurrentSettings.UseStreamComparer(compare);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseStringComparer(StringCompare)"/>
    [Pure]
    [Obsolete("Use UseStringComparer(StringCompare compare, params ReadOnlySpan<string> extensions)")]

    public SettingsTask UseStringComparer(StringCompare compare)
    {
        CurrentSettings.UseStringComparer(compare);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AutoVerify(bool)"/>
    [Obsolete("Use SettingsTask.AutoVerify(bool includeBuildServer, bool throwException)")]
    [Pure]
    public SettingsTask AutoVerify(bool includeBuildServer = true)
    {
        CurrentSettings.AutoVerify(includeBuildServer, false);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AutoVerify(VerifyTests.AutoVerify, bool)"/>
    [Obsolete("Use SettingsTask.AutoVerify(AutoVerify, autoVerify, bool includeBuildServer, bool throwException)")]
    [Pure]
    public SettingsTask AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true)
    {
        CurrentSettings.AutoVerify(autoVerify, includeBuildServer, false);
        return this;
    }
}
