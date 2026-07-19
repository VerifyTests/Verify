namespace VerifyTests;

public partial class VerifySettings
{
    const string locationObsolete = "ScrubberLocation is ignored; span scrubber ordering is engine determined. Use the overload without ScrubberLocation.";

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubMachineName(ScrubberLocation location) =>
        ScrubMachineName();

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubUserName(ScrubberLocation location) =>
        ScrubUserName();

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, stringToMatch);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubInlineGuids(ScrubberLocation location) =>
        ScrubInlineGuids();

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
    [Obsolete(locationObsolete)]
    public void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
        string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimeOffsets(format, culture);

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
    [Obsolete(locationObsolete)]
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(replaceLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubEmptyLines(ScrubberLocation location) =>
        ScrubEmptyLines();

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesContaining(ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

#if NET6_0_OR_GREATER

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
}
