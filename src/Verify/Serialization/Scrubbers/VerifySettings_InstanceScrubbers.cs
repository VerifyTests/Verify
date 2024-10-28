namespace VerifyTests;

public partial class VerifySettings
{
    internal List<Action<StringBuilder, Counter>> InstanceScrubbers = [];

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public VerifySettings ScrubMachineName(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(Scrubbers.ScrubMachineName, location);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(Scrubbers.ScrubUserName, location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public VerifySettings AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public VerifySettings AddScrubber(Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        switch (location)
        {
            case ScrubberLocation.First:
                InstanceScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                InstanceScrubbers.Add(scrubber);
                break;
        }

        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        AddScrubber(_ => _.RemoveLinesContaining(comparison, stringToMatch), location);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public VerifySettings ScrubInlineGuids(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(GuidScrubber.ReplaceGuids, location);

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    public VerifySettings ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null,
        ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(
            DateScrubber.BuildDateTimeScrubber(format, culture),
            location);

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    public VerifySettings ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null,
        ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(
            DateScrubber.BuildDateTimeOffsetScrubber(format, culture),
            location);

#if NET5_0_OR_GREATER

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    public VerifySettings ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture = null,
        ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(
            DateScrubber.BuildDateScrubber(format, culture),
            location);

#endif

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLines(Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.FilterLines(removeLine), location);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public VerifySettings ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.ReplaceLines(replaceLine), location);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public VerifySettings ScrubEmptyLines(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.RemoveEmptyLines(), location);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, location, stringToMatch);
}