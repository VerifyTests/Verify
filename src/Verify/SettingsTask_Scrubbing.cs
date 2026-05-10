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

    /// <inheritdoc cref="VerifySettings.AddScrubber(PatternScrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(PatternScrubber scrubber)
    {
        CurrentSettings.AddScrubber(scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(LineScrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(LineScrubber scrubber)
    {
        CurrentSettings.AddScrubber(scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(ContentScrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(ContentScrubber scrubber)
    {
        CurrentSettings.AddScrubber(scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(string, PatternScrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(string extension, PatternScrubber scrubber)
    {
        CurrentSettings.AddScrubber(extension, scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(string, LineScrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(string extension, LineScrubber scrubber)
    {
        CurrentSettings.AddScrubber(extension, scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(string, ContentScrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(string extension, ContentScrubber scrubber)
    {
        CurrentSettings.AddScrubber(extension, scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids()"/>
    [Pure]
    public SettingsTask ScrubInlineGuids()
    {
        CurrentSettings.ScrubInlineGuids();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubGuids()"/>
    [Pure]
    public SettingsTask ScrubGuids()
    {
        CurrentSettings.ScrubGuids();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubNumericIds()"/>
    [Pure]
    public SettingsTask ScrubNumericIds()
    {
        CurrentSettings.ScrubNumericIds();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids(string)"/>
    [Pure]
    public SettingsTask ScrubInlineGuids(string extension)
    {
        CurrentSettings.ScrubInlineGuids(extension);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DisableDateCounting()"/>
    [Pure]
    public SettingsTask DisableDateCounting()
    {
        CurrentSettings.DisableDateCounting();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubDateTimes()"/>
    [Pure]
    public SettingsTask ScrubDateTimes()
    {
        CurrentSettings.ScrubDateTimes();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimeOffsets(string,Culture?)"/>
    [Pure]
    public SettingsTask ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null)
    {
        CurrentSettings.ScrubInlineDateTimeOffsets(format, culture);
        return this;
    }

#if NET6_0_OR_GREATER

    /// <inheritdoc cref="VerifySettings.ScrubInlineDates(string,Culture?)"/>
    [Pure]
    public SettingsTask ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture = null)
    {
        CurrentSettings.ScrubInlineDates(format, culture);
        return this;
    }

#endif

    /// <inheritdoc cref="VerifySettings.ScrubMachineName()"/>
    [Pure]
    public SettingsTask ScrubMachineName()
    {
        CurrentSettings.ScrubMachineName();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMachineName(string)"/>
    [Pure]
    public SettingsTask ScrubMachineName(string extension)
    {
        CurrentSettings.ScrubMachineName(extension);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubUserName()"/>
    [Pure]
    public SettingsTask ScrubUserName()
    {
        CurrentSettings.ScrubUserName();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubUserName(string)"/>
    [Pure]
    public SettingsTask ScrubUserName(string extension)
    {
        CurrentSettings.ScrubUserName(extension);
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

    /// <inheritdoc cref="VerifySettings.ScrubLines(Func{string,bool})"/>
    [Pure]
    public SettingsTask ScrubLines(Func<string, bool> removeLine)
    {
        CurrentSettings.ScrubLines(removeLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLines(string,Func{string,bool})"/>
    [Pure]
    public SettingsTask ScrubLines(string extension, Func<string, bool> removeLine)
    {
        CurrentSettings.ScrubLines(extension, removeLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(Func{string,string?})"/>
    [Pure]
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(string,Func{string,string?})"/>
    [Pure]
    public SettingsTask ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(extension, replaceLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines()"/>
    [Pure]
    public SettingsTask ScrubEmptyLines()
    {
        CurrentSettings.ScrubEmptyLines();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines(string)"/>
    [Pure]
    public SettingsTask ScrubEmptyLines(string extension)
    {
        CurrentSettings.ScrubEmptyLines(extension);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(stringToMatch);
        return this;
    }
}
