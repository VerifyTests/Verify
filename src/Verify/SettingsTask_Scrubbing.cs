namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.DisableScrubbers()"/>
    [Pure]
    public SettingsTask DisableScrubbers()
    {
        CurrentSettings.DisableScrubbers();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(Action{StringBuilder},VerifyTests.ScrubberLocation)"/>
    [Pure]
    public SettingsTask AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddScrubber(scrubber, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(string, Action{StringBuilder},VerifyTests.ScrubberLocation)"/>
    [Pure]
    public SettingsTask AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddScrubber(extension, scrubber, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids(ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubInlineGuids(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubInlineGuids(location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids(string,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubInlineGuids(string extension, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubInlineGuids(extension, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DisableDateCounting()"/>
    [Pure]
    public SettingsTask DisableDateCounting()
    {
        CurrentSettings.DisableDateCounting();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimes(string,Culture?,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null,
        ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubInlineDateTimes(format, culture, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimeOffsets(string,Culture?,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null,
        ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubInlineDateTimeOffsets(format, culture, location);
        return this;
    }

#if NET6_0_OR_GREATER

    /// <inheritdoc cref="VerifySettings.ScrubInlineDates(string,Culture?,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture = null,
        ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubInlineDates(format, culture, location);
        return this;
    }

#endif

    /// <inheritdoc cref="VerifySettings.ScrubMachineName(ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubMachineName(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubMachineName(location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMachineName(string,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubMachineName(string extension, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubMachineName(extension, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubUserName(ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubUserName(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubUserName(location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubUserName(string,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubUserName(string extension,ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubUserName(extension, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(StringComparison,string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(comparison, stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string,StringComparison,string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(extension, comparison, stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(StringComparison,ScrubberLocation,string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(StringComparison comparison, ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(comparison, location, stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string,StringComparison,ScrubberLocation,string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(extension, comparison, location, stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string,StringComparison,ScrubberLocation,string[])"/>
    [Pure]
    public SettingsTask ScrubLines(Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubLines(removeLine, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLines(Func{string,bool},ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubLines(extension, removeLine, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(Func{string,string?},ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(string,Func{string,string?},ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubLinesWithReplace(extension, replaceLine, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines(ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubEmptyLines(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubEmptyLines(location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines(string,ScrubberLocation)"/>
    [Pure]
    public SettingsTask ScrubEmptyLines(string extension, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubEmptyLines(extension, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(StringComparison,string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(location, stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string,ScrubberLocation,string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(string extension, ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(extension, location, stringToMatch);
        return this;
    }
}