namespace VerifyTests;

public partial class VerifySettings
{
    internal List<Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>>>? InstanceScrubbers = [];

    internal List<Scrubber>? InstanceSpanScrubbers;

    const string locationObsolete = "ScrubberLocation is ignored; span scrubber ordering is engine determined. Use the overload without ScrubberLocation.";

    /// <summary>
    /// Add a <see cref="Scrubber" />.
    /// </summary>
    public void AddScrubber(Scrubber scrubber)
    {
        Ensure.NotNull(scrubber);
        InstanceSpanScrubbers ??= [];
        InstanceSpanScrubbers.Add(scrubber);
    }

    internal bool ScrubbersEnabled { get; private set; } = true;

    /// <summary>
    /// Disables all scrubbers.
    /// </summary>
    public void DisableScrubbers() => ScrubbersEnabled = false;

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName() =>
        AddScrubber(UserMachineScrubber.MachineScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubMachineName(ScrubberLocation location) =>
        ScrubMachineName();

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName() =>
        AddScrubber(UserMachineScrubber.UserScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubUserName(ScrubberLocation location) =>
        ScrubUserName();

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, counter, _) =>
            scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        if (InstanceScrubbers == null)
        {
            InstanceScrubbers = [scrubber];
            return;
        }

        switch (location)
        {
            case ScrubberLocation.First:
                InstanceScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                InstanceScrubbers.Add(scrubber);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch) =>
        AddScrubber(Scrubber.RemoveLinesContaining(comparison, stringToMatch));

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
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

        AddScrubber(GuidMatcher.Instance);
    }

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
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

        foreach (var scrubber in DateMatchers.DateTimes(format, culture))
        {
            AddScrubber(scrubber);
        }
    }

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
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

        foreach (var scrubber in DateMatchers.DateTimeOffsets(format, culture))
        {
            AddScrubber(scrubber);
        }
    }

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
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

        foreach (var scrubber in DateMatchers.Dates(format, culture))
        {
            AddScrubber(scrubber);
        }
    }

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
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
        AddScrubber(Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// No per line string is allocated for span predicates.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ScrubLines((CharSpan line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public void ScrubLines(LineMatch removeLine) =>
        AddScrubber(Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(removeLine);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        AddScrubber(Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// No per line string is allocated for span based replacers.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ScrubLinesWithReplace((CharSpan line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public void ScrubLinesWithReplace(LineReplace replaceLine) =>
        AddScrubber(Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(replaceLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines() =>
        AddScrubber(Scrubber.RemoveEmptyLines());

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
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
    [Obsolete(locationObsolete)]
    public void ScrubLinesContaining(ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
}
