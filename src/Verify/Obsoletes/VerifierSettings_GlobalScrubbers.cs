// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyTests;

public static partial class VerifierSettings
{
    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public static void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, _, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public static void AddScrubber(Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, counter, _) => scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(ContentScrubber). See the scrubber migration guide.")]
    public static void AddScrubber(Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var adapter = new LambdaContentScrubber(scrubber);
        switch (location)
        {
            case ScrubberLocation.First:
                GlobalContentScrubbers.Insert(0, adapter);
                break;
            case ScrubberLocation.Last:
                GlobalContentScrubbers.Add(adapter);
                break;
        }
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(StringComparison, params string[]).")]
    public static void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, stringToMatch);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(LineFilter).")]
    public static void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(line => removeLine(line.ToString()));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete("Use ScrubLines(LineFilter)")]
    public static void ScrubLines(Func<string, bool> removeLine) =>
        ScrubLines(line => removeLine(line.ToString()));

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines().")]
    public static void ScrubEmptyLines(ScrubberLocation location) =>
        ScrubEmptyLines();

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDateTimes(string, Culture?).")]
    public static void ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimes(format, culture);

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDateTimeOffsets(string, Culture?).")]
    public static void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimeOffsets(format, culture);

#if NET6_0_OR_GREATER

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineDates(string, Culture?).")]
    public static void ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDates(format, culture);

#endif

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids().")]
    public static void ScrubInlineGuids(ScrubberLocation location) =>
        ScrubInlineGuids();

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(LineReplace).")]
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace((line, out r) =>
        {
            var result = replaceLine(line.ToString());
            if (result is null)
            {
                r = default;
                return false;
            }

            r = result.AsSpan();
            return true;
        });

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    [Obsolete("Use ScrubLinesWithReplace(LineReplace)")]
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        ScrubLinesWithReplace((line, out r) =>
        {
            var result = replaceLine(line.ToString());
            if (result is null)
            {
                r = default;
                return false;
            }

            r = result.AsSpan();
            return true;
        });

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(params string[]).")]
    public static void ScrubLinesContaining(ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName().")]
    public static void ScrubMachineName(ScrubberLocation location) =>
        ScrubMachineName();

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName().")]
    public static void ScrubUserName(ScrubberLocation location) =>
        ScrubUserName();
}
