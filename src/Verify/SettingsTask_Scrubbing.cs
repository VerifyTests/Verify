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

    [Pure]
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public SettingsTask AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddScrubber(scrubber, location);
        return this;
    }

    [Pure]
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public SettingsTask AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddScrubber(extension, scrubber, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids()"/>
    [Pure]
    public SettingsTask ScrubInlineGuids()
    {
        CurrentSettings.ScrubInlineGuids();
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids().")]
    public SettingsTask ScrubInlineGuids(ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids(string).")]
    public SettingsTask ScrubInlineGuids(string extension, ScrubberLocation location)
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

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimes(string,Culture?)"/>
    [Pure]
    public SettingsTask ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null)
    {
        CurrentSettings.ScrubInlineDateTimes(format, culture);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDateTimes(string, Culture?).")]
    public SettingsTask ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location)
    {
        CurrentSettings.ScrubInlineDateTimes(format, culture);
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDateTimeOffsets(string, Culture?).")]
    public SettingsTask ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDates(string, Culture?).")]
    public SettingsTask ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture,
        ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName().")]
    public SettingsTask ScrubMachineName(ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName(string).")]
    public SettingsTask ScrubMachineName(string extension, ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName().")]
    public SettingsTask ScrubUserName(ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName(string).")]
    public SettingsTask ScrubUserName(string extension, ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(Func<string, bool>).")]
    public SettingsTask ScrubLines(Func<string, bool> removeLine, ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(string, Func<string, bool>).")]
    public SettingsTask ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(Func<string, string?>).")]
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(string, Func<string, string?>).")]
    public SettingsTask ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines().")]
    public SettingsTask ScrubEmptyLines(ScrubberLocation location)
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines(string).")]
    public SettingsTask ScrubEmptyLines(string extension, ScrubberLocation location)
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
