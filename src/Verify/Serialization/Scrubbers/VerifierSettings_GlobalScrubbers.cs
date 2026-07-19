namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static List<Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>>> GlobalScrubbers = [];

    internal static List<Scrubber> GlobalSpanScrubbers = [];

    /// <summary>
    /// Add a <see cref="Scrubber" /> that applies to all verified files.
    /// </summary>
    internal static void AddScrubber(Scrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
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
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(Func<string, bool> removeLine) =>
        AddScrubber(Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// No per line string is allocated for span predicates.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ScrubLines((ReadOnlySpan&lt;char&gt; line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public static void ScrubLines(LineMatch removeLine) =>
        AddScrubber(Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines() =>
        AddScrubber(Scrubber.RemoveEmptyLines());

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

#endif

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids() =>
        AddScrubber(GuidMatcher.Instance);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        AddScrubber(Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// No per line string is allocated for span based replacers.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ScrubLinesWithReplace((ReadOnlySpan&lt;char&gt; line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public static void ScrubLinesWithReplace(LineReplace replaceLine) =>
        AddScrubber(Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName() =>
        AddScrubber(UserMachineScrubber.MachineScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName() =>
        AddScrubber(UserMachineScrubber.UserScrubber());

    /// <summary>
    /// Replace every occurrence of <paramref name="find" /> with <paramref name="replacement" />.
    /// <paramref name="comparison" /> must be <see cref="StringComparison.Ordinal" /> or <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static void ScrubReplace(string find, string replacement, StringComparison comparison = StringComparison.Ordinal, bool requireWordBoundary = false) =>
        AddScrubber(Scrubber.Replace(find, replacement, comparison, requireWordBoundary));

    /// <summary>
    /// Replace every occurrence of each Find with its Replacement.
    /// At a given position the longest matching Find wins.
    /// <paramref name="comparison" /> must be <see cref="StringComparison.Ordinal" /> or <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static void ScrubReplace(StringComparison comparison, bool requireWordBoundary, params (string Find, string Replacement)[] pairs) =>
        AddScrubber(Scrubber.Replace(comparison, requireWordBoundary, pairs));

    /// <summary>
    /// Match candidate windows of text between <paramref name="minLength" /> and <paramref name="maxLength" /> characters.
    /// At each position the engine tries the longest window first.
    /// </summary>
    public static void ScrubWindow(int minLength, int maxLength, WindowMatch matcher, bool requireWordBoundary = false) =>
        AddScrubber(Scrubber.Window(minLength, maxLength, matcher, requireWordBoundary));

    /// <summary>
    /// Find matches using custom search logic.
    /// <paramref name="minLength" />: segments shorter than this are skipped (null scans everything).
    /// <paramref name="maxLength" />: used for ordering only; null (unknown) runs before all known length scrubbers.
    /// </summary>
    public static void ScrubMatch(SegmentMatch matcher, int? minLength = null, int? maxLength = null) =>
        AddScrubber(Scrubber.Match(matcher, minLength, maxLength));
}
