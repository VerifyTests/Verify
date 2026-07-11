namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static List<Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>>> GlobalScrubbers = [];

    internal static List<Scrubber> GlobalSpanScrubbers = [];

    const string locationObsolete = "ScrubberLocation is ignored; span scrubber ordering is engine determined. Use the overload without ScrubberLocation.";

    /// <summary>
    /// Add a <see cref="Scrubber" /> that applies to all verified files.
    /// </summary>
    public static void AddScrubber(Scrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Ensure.NotNull(scrubber);
        GlobalSpanScrubbers.Add(scrubber);
        EngineScrubberSet.InvalidateGlobalCache();
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, _, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber((builder, counter, _) => scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
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
    public static void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch) =>
        AddScrubber(Scrubber.RemoveLinesContaining(comparison, stringToMatch));

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, stringToMatch);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(Func<string, bool> removeLine) =>
        AddScrubber(Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(removeLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines() =>
        AddScrubber(Scrubber.RemoveEmptyLines());

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubEmptyLines(ScrubberLocation location) =>
        ScrubEmptyLines();

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
        Culture? culture = null)
    {
        foreach (var scrubber in DateMatchers.DateTimes(format, culture))
        {
            AddScrubber(scrubber);
        }
    }

    /// <summary>
    /// Replace inline <see cref="DateTime" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubInlineDateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimes(format, culture);

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture = null)
    {
        foreach (var scrubber in DateMatchers.DateTimeOffsets(format, culture))
        {
            AddScrubber(scrubber);
        }
    }

    /// <summary>
    /// Replace inline <see cref="DateTimeOffset" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubInlineDateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDateTimeOffsets(format, culture);

#if NET6_0_OR_GREATER

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture = null)
    {
        foreach (var scrubber in DateMatchers.Dates(format, culture))
        {
            AddScrubber(scrubber);
        }
    }

    /// <summary>
    /// Replace inline <see cref="Date" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubInlineDates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture,
        ScrubberLocation location) =>
        ScrubInlineDates(format, culture);

#endif

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids() =>
        AddScrubber(GuidMatcher.Instance);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubInlineGuids(ScrubberLocation location) =>
        ScrubInlineGuids();

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        AddScrubber(Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(replaceLine);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubLinesContaining(ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName() =>
        AddScrubber(UserMachineScrubber.MachineScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubMachineName(ScrubberLocation location) =>
        ScrubMachineName();

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName() =>
        AddScrubber(UserMachineScrubber.UserScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public static void ScrubUserName(ScrubberLocation location) =>
        ScrubUserName();
}
