namespace VerifyTests;

public partial class VerifySettings
{
    internal List<PatternScrubber>? InstancePatternScrubbers;
    internal List<LineScrubber>? InstanceLineScrubbers;
    internal List<ContentScrubber>? InstanceContentScrubbers;

    internal bool ScrubbersEnabled { get; private set; } = true;

    /// <summary>
    /// Disables all scrubbers.
    /// </summary>
    public void DisableScrubbers() => ScrubbersEnabled = false;

    /// <summary>
    /// Register a <see cref="PatternScrubber" /> for this verification.
    /// </summary>
    public void AddScrubber(PatternScrubber scrubber)
    {
        InstancePatternScrubbers ??= [];
        InstancePatternScrubbers.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="LineScrubber" /> for this verification.
    /// </summary>
    public void AddScrubber(LineScrubber scrubber)
    {
        InstanceLineScrubbers ??= [];
        InstanceLineScrubbers.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="ContentScrubber" /> for this verification.
    /// </summary>
    public void AddScrubber(ContentScrubber scrubber)
    {
        InstanceContentScrubbers ??= [];
        InstanceContentScrubbers.Add(scrubber);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, _, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public void AddScrubber(Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, counter, _) => scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public void AddScrubber(Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InstanceContentScrubbers ??= [];
        var adapter = new ActionAdapterContentScrubber(scrubber);
        switch (location)
        {
            case ScrubberLocation.First:
                InstanceContentScrubbers.Insert(0, adapter);
                break;
            case ScrubberLocation.Last:
                InstanceContentScrubbers.Add(adapter);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName() =>
        AddScrubber(UserMachineScrubber.MachinePatternScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName().")]
    public void ScrubMachineName(ScrubberLocation location) =>
        ScrubMachineName();

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName() =>
        AddScrubber(UserMachineScrubber.UserPatternScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName().")]
    public void ScrubUserName(ScrubberLocation location) =>
        ScrubUserName();

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        Ensure.NotNullOrEmpty(stringToMatch);
        AddScrubber(new RemoveLinesContainingScrubber(comparison, stringToMatch));
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(StringComparison, params string[]).")]
    public void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, stringToMatch);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public void ScrubInlineGuids()
    {
        if (serialization.ScrubGuids == false)
        {
            throw new("ScrubGuids is disabled. Call .ScrubGuids() before calling .ScrubInlineGuids().");
        }

        AddScrubber(GuidPatternScrubber.Instance);
    }

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids().")]
    public void ScrubInlineGuids(ScrubberLocation location) =>
        ScrubInlineGuids();

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    public void ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
        string format,
        Culture? culture = null)
    {
        if (serialization.ScrubDateTimes == false)
        {
            throw new("ScrubDateTimes is disabled. Call .ScrubDateTimes() before calling .ScrubInlineDateTimes().");
        }

        AddScrubber(new DateTimePatternScrubber(format, culture));
    }

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDateTimes(string, Culture?).")]
    public void ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
        string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimes(format, culture);

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    public void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
        string format,
        Culture? culture = null)
    {
        if (serialization.ScrubDateTimes == false)
        {
            throw new("ScrubDateTimes is disabled. Call .ScrubDateTimes() before calling .ScrubInlineDateTimeOffsets().");
        }

        AddScrubber(new DateTimeOffsetPatternScrubber(format, culture));
    }

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDateTimeOffsets(string, Culture?).")]
    public void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
        string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimeOffsets(format, culture);

#if NET6_0_OR_GREATER

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    public void ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture = null)
    {
        if (serialization.ScrubDateTimes == false)
        {
            throw new("ScrubDateTimes is disabled. Call .ScrubDateTimes() before calling .ScrubInlineDates().");
        }

        AddScrubber(new DatePatternScrubber(format, culture));
    }

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDates(string, Culture?).")]
    public void ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDates(format, culture);

#endif

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public void ScrubLines(Func<string, bool> removeLine) =>
        AddScrubber(new FilterLinesScrubber(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(Func<string, bool>).")]
    public void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(removeLine);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        AddScrubber(new ReplaceLinesScrubber(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(Func<string, string?>).")]
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(replaceLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines() =>
        AddScrubber(RemoveEmptyLinesScrubber.Instance);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines().")]
    public void ScrubEmptyLines(ScrubberLocation location) =>
        ScrubEmptyLines();

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(params string[]).")]
    public void ScrubLinesContaining(ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
}
