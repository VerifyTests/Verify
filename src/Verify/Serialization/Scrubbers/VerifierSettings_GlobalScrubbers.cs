namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static List<Action<StringBuilder, Counter>> GlobalScrubbers = [];

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber((builder, _) => scrubber(builder), location);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        switch (location)
        {
            case ScrubberLocation.First:
                GlobalScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                GlobalScrubbers.Add(scrubber);
                break;
        }
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        ScrubLinesContaining(comparison, ScrubberLocation.First, stringToMatch);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(_ => _.RemoveLinesContaining(comparison, stringToMatch), location);
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(_ => _.FilterLines(removeLine), location);
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines(ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(_ => _.RemoveEmptyLines(), location);
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
    public static void ScrubInlineDateTimes(string format, Culture? culture = null, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(
            DateScrubber.BuildDateTimeScrubber(format, culture),
            location);

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDateTimeOffsets(string format, Culture? culture = null, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(
            DateScrubber.BuildDateTimeOffsetScrubber(format, culture),
            location);

#if NET5_0_OR_GREATER

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDates(string format, Culture? culture = null, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(
            DateScrubber.BuildDateScrubber(format, culture),
            location);

#endif

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids(ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(GuidScrubber.ReplaceGuids, location);
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(_ => _.ReplaceLines(replaceLine), location);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(params string[] stringToMatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        ScrubLinesContaining(ScrubberLocation.First, stringToMatch);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, location, stringToMatch);
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName(ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(Scrubbers.ScrubMachineName, location);
    }

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName(ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(Scrubbers.ScrubUserName, location);
    }
}