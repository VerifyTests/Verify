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

    /// <inheritdoc cref="VerifySettings.ScrubInlineDateTimes(string,Culture?)"/>
    [Pure]
    public SettingsTask ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null)
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

    /// <inheritdoc cref="VerifySettings.ScrubLines(LineMatch)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLines(LineMatch removeLine)
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

    /// <inheritdoc cref="VerifySettings.ScrubLines(string,LineMatch)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLines(string extension, LineMatch removeLine)
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

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(LineReplace)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLinesWithReplace(LineReplace replaceLine)
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

    /// <inheritdoc cref="VerifySettings.ScrubLinesWithReplace(string,LineReplace)"/>
    [OverloadResolutionPriority(-1)]
    [Pure]
    public SettingsTask ScrubLinesWithReplace(string extension, LineReplace replaceLine)
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

    /// <inheritdoc cref="VerifySettings.ScrubReplace(string,string,StringComparison,bool)"/>
    [Pure]
    public SettingsTask ScrubReplace(string find, string replacement, StringComparison comparison = StringComparison.Ordinal, bool requireWordBoundary = false)
    {
        CurrentSettings.ScrubReplace(find, replacement, comparison, requireWordBoundary);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubReplace(string,string,string,StringComparison,bool)"/>
    [Pure]
    public SettingsTask ScrubReplace(string extension, string find, string replacement, StringComparison comparison = StringComparison.Ordinal, bool requireWordBoundary = false)
    {
        CurrentSettings.ScrubReplace(extension, find, replacement, comparison, requireWordBoundary);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubReplace(StringComparison,bool,ValueTuple{string,string}[])"/>
    [Pure]
    public SettingsTask ScrubReplace(StringComparison comparison, bool requireWordBoundary, params (string Find, string Replacement)[] pairs)
    {
        CurrentSettings.ScrubReplace(comparison, requireWordBoundary, pairs);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubReplace(string,StringComparison,bool,ValueTuple{string,string}[])"/>
    [Pure]
    public SettingsTask ScrubReplace(string extension, StringComparison comparison, bool requireWordBoundary, params (string Find, string Replacement)[] pairs)
    {
        CurrentSettings.ScrubReplace(extension, comparison, requireWordBoundary, pairs);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubWindow(int,int,WindowMatch,bool)"/>
    [Pure]
    public SettingsTask ScrubWindow(int minLength, int maxLength, WindowMatch matcher, bool requireWordBoundary = false)
    {
        CurrentSettings.ScrubWindow(minLength, maxLength, matcher, requireWordBoundary);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubWindow(string,int,int,WindowMatch,bool)"/>
    [Pure]
    public SettingsTask ScrubWindow(string extension, int minLength, int maxLength, WindowMatch matcher, bool requireWordBoundary = false)
    {
        CurrentSettings.ScrubWindow(extension, minLength, maxLength, matcher, requireWordBoundary);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMatch(SegmentMatch,int?,int?)"/>
    [Pure]
    public SettingsTask ScrubMatch(SegmentMatch matcher, int? minLength = null, int? maxLength = null)
    {
        CurrentSettings.ScrubMatch(matcher, minLength, maxLength);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMatch(string,SegmentMatch,int?,int?)"/>
    [Pure]
    public SettingsTask ScrubMatch(string extension, SegmentMatch matcher, int? minLength = null, int? maxLength = null)
    {
        CurrentSettings.ScrubMatch(extension, matcher, minLength, maxLength);
        return this;
    }
}
