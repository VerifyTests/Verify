// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static List<PatternScrubber> GlobalPatternScrubbers = [];
    internal static List<LineScrubber> GlobalLineScrubbers = [];
    internal static List<ContentScrubber> GlobalContentScrubbers = [];

    internal static void ClearAllGlobalScrubbers()
    {
        GlobalPatternScrubbers.Clear();
        GlobalLineScrubbers.Clear();
        GlobalContentScrubbers.Clear();
        ExtensionMappedGlobalPatternScrubbers.Clear();
        ExtensionMappedGlobalLineScrubbers.Clear();
        ExtensionMappedGlobalContentScrubbers.Clear();
    }

    /// <summary>
    /// Register a <see cref="PatternScrubber" /> that runs over every verification.
    /// Pattern scrubbers are sorted by <see cref="PatternScrubber.MaxLength" /> descending
    /// so longer/more-specific matches win overlapping ranges.
    /// </summary>
    public static void AddScrubber(PatternScrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        GlobalPatternScrubbers.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="LineScrubber" /> that runs per-line over every verification.
    /// Line scrubbers run after pattern scrubbers, in registration order.
    /// </summary>
    public static void AddScrubber(LineScrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        GlobalLineScrubbers.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="ContentScrubber" /> that runs over the full content of every verification.
    /// Content scrubbers run before pattern and line scrubbers, in registration order.
    /// </summary>
    public static void AddScrubber(ContentScrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        GlobalContentScrubbers.Add(scrubber);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Ensure.NotNullOrEmpty(stringToMatch);
        AddScrubber(new RemoveLinesContainingScrubber(comparison, stringToMatch));
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(Func<string, bool> removeLine)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(new FilterLinesScrubber(removeLine));
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(RemoveEmptyLinesScrubber.Instance);
    }

    internal static bool DateCountingEnabled { get; private set; } = true;

    /// <summary>
    /// Disables counting of dates.
    /// </summary>
    public static void DisableDateCounting() =>
        DateCountingEnabled = false;

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null) =>
        AddScrubber(new DateTimePatternScrubber(format, culture));

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null) =>
        AddScrubber(new DateTimeOffsetPatternScrubber(format, culture));

#if NET6_0_OR_GREATER

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture = null) =>
        AddScrubber(new DatePatternScrubber(format, culture));

#endif

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(GuidPatternScrubber.Instance);
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(new ReplaceLinesScrubber(replaceLine));
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(UserMachineScrubber.MachinePatternScrubber());
    }

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(UserMachineScrubber.UserPatternScrubber());
    }
}
