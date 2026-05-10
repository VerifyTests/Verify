namespace VerifyTests;

public partial class SettingsTask
{
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids().")]
    public SettingsTask ScrubInlineGuids(ScrubberLocation location)
    {
        CurrentSettings.ScrubInlineGuids();
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids(string).")]
    public SettingsTask ScrubInlineGuids(string extension, ScrubberLocation location)
    {
        CurrentSettings.ScrubInlineGuids(extension);
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

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName().")]
    public SettingsTask ScrubMachineName(ScrubberLocation location)
    {
        CurrentSettings.ScrubMachineName();
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName(string).")]
    public SettingsTask ScrubMachineName(string extension, ScrubberLocation location)
    {
        CurrentSettings.ScrubMachineName(extension);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName().")]
    public SettingsTask ScrubUserName(ScrubberLocation location)
    {
        CurrentSettings.ScrubUserName();
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName(string).")]
    public SettingsTask ScrubUserName(string extension, ScrubberLocation location)
    {
        CurrentSettings.ScrubUserName(extension);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(Func<string, bool>).")]
    public SettingsTask ScrubLines(Func<string, bool> removeLine, ScrubberLocation location)
    {
        CurrentSettings.ScrubLines(removeLine);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(string, Func<string, bool>).")]
    public SettingsTask ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location)
    {
        CurrentSettings.ScrubLines(extension, removeLine);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(Func<string, string?>).")]
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(string, Func<string, string?>).")]
    public SettingsTask ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location)
    {
        CurrentSettings.ScrubLinesWithReplace(extension, replaceLine);
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines().")]
    public SettingsTask ScrubEmptyLines(ScrubberLocation location)
    {
        CurrentSettings.ScrubEmptyLines();
        return this;
    }

    [Pure]
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines(string).")]
    public SettingsTask ScrubEmptyLines(string extension, ScrubberLocation location)
    {
        CurrentSettings.ScrubEmptyLines(extension);
        return this;
    }
}
