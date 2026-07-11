namespace VerifyTests;

public partial class SettingsTask
{
    const string locationObsolete = "ScrubberLocation is ignored; span scrubber ordering is engine determined. Use the overload without ScrubberLocation.";

    /// <inheritdoc cref="VerifySettings.DisableScrubbers()"/>
    [Pure]
    public SettingsTask DisableScrubbers()
    {
        CurrentSettings.DisableScrubbers();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(Scrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(Scrubber scrubber)
    {
        CurrentSettings.AddScrubber(scrubber);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddScrubber(string,Scrubber)"/>
    [Pure]
    public SettingsTask AddScrubber(string extension, Scrubber scrubber)
    {
        CurrentSettings.AddScrubber(extension, scrubber);
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

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids()"/>
    [Pure]
    public SettingsTask ScrubInlineGuids()
    {
        CurrentSettings.ScrubInlineGuids();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids()"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubInlineGuids(ScrubberLocation location) =>
        ScrubInlineGuids();

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

    /// <inheritdoc cref="VerifySettings.ScrubInlineGuids(string)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubInlineGuids(string extension, ScrubberLocation location) =>
        ScrubInlineGuids(extension);

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

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimes(string,Culture?)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimes(format, culture);

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimeOffsets(string,Culture?)"/>
    [Pure]
    public SettingsTask ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null)
    {
        CurrentSettings.ScrubInlineDateTimeOffsets(format, culture);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimeOffsets(string,Culture?)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimeOffsets(format, culture);

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

    /// <inheritdoc cref="VerifySettings.ScrubInlineDates(string,Culture?)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDates(format, culture);

#endif

    /// <inheritdoc cref="VerifySettings.ScrubMachineName()"/>
    [Pure]
    public SettingsTask ScrubMachineName()
    {
        CurrentSettings.ScrubMachineName();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMachineName()"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubMachineName(ScrubberLocation location) =>
        ScrubMachineName();

    /// <inheritdoc cref="VerifySettings.ScrubMachineName(string)"/>
    [Pure]
    public SettingsTask ScrubMachineName(string extension)
    {
        CurrentSettings.ScrubMachineName(extension);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMachineName(string)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubMachineName(string extension, ScrubberLocation location) =>
        ScrubMachineName(extension);

    /// <inheritdoc cref="VerifySettings.ScrubUserName()"/>
    [Pure]
    public SettingsTask ScrubUserName()
    {
        CurrentSettings.ScrubUserName();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubUserName()"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubUserName(ScrubberLocation location) =>
        ScrubUserName();

    /// <inheritdoc cref="VerifySettings.ScrubUserName(string)"/>
    [Pure]
    public SettingsTask ScrubUserName(string extension)
    {
        CurrentSettings.ScrubUserName(extension);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubUserName(string)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubUserName(string extension, ScrubberLocation location) =>
        ScrubUserName(extension);

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

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(StringComparison,string[])"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, stringToMatch);

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string,StringComparison,string[])"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, comparison, stringToMatch);

    /// <inheritdoc cref="VerifySettings.ScrubLines(Func{string,bool})"/>
    [Pure]
    public SettingsTask ScrubLines(Func<string, bool> removeLine)
    {
        CurrentSettings.ScrubLines(removeLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLines(LineMatch)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLines(LineMatch removeLine)
    {
        CurrentSettings.ScrubLines(removeLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLines(Func{string,bool})"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLines(Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(removeLine);

    /// <inheritdoc cref="VerifySettings.ScrubLines(string,Func{string,bool})"/>
    [Pure]
    public SettingsTask ScrubLines(string extension, Func<string, bool> removeLine)
    {
        CurrentSettings.ScrubLines(extension, removeLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLines(string,LineMatch)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLines(string extension, LineMatch removeLine)
    {
        CurrentSettings.ScrubLines(extension, removeLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLines(string,Func{string,bool})"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(extension, removeLine);

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(Func{string,string?})"/>
    [Pure]
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(LineReplace)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLinesWithReplace(LineReplace replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(Func{string,string?})"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(replaceLine);

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(string,Func{string,string?})"/>
    [Pure]
    public SettingsTask ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(extension, replaceLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(string,LineReplace)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLinesWithReplace(string extension, LineReplace replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(extension, replaceLine);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(string,Func{string,string?})"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(extension, replaceLine);

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines()"/>
    [Pure]
    public SettingsTask ScrubEmptyLines()
    {
        CurrentSettings.ScrubEmptyLines();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines()"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubEmptyLines(ScrubberLocation location) =>
        ScrubEmptyLines();

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines(string)"/>
    [Pure]
    public SettingsTask ScrubEmptyLines(string extension)
    {
        CurrentSettings.ScrubEmptyLines(extension);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubEmptyLines(string)"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubEmptyLines(string extension, ScrubberLocation location) =>
        ScrubEmptyLines(extension);

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string[])"/>
    [Pure]
    public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(stringToMatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string[])"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLinesContaining(ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(stringToMatch);

    /// <inheritdoc cref="VerifySettings.ScrubLinesContaining(string,StringComparison,string[])"/>
    [Obsolete(locationObsolete)]
    [Pure]
    public SettingsTask ScrubLinesContaining(string extension, ScrubberLocation location, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(extension, StringComparison.OrdinalIgnoreCase, stringToMatch);
        return this;
    }
}
