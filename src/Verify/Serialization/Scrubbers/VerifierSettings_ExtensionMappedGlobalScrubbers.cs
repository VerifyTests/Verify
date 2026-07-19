namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Dictionary<string, List<Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>>>> ExtensionMappedGlobalScrubbers = [];

    internal static Dictionary<string, List<Scrubber>> ExtensionMappedGlobalSpanScrubbers = [];

    /// <summary>
    /// Add a <see cref="Scrubber" /> that applies to all verified files with a matching extension.
    /// </summary>
    public static void AddScrubber(string extension, Scrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Ensure.NotNull(scrubber);
        if (!ExtensionMappedGlobalSpanScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalSpanScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
        EngineScrubberSet.InvalidateGlobalCache();
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, (builder, _) => scrubber(builder), location);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(string extension, Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, counter, _) =>
            scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(string extension, Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (!ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalScrubbers[extension] = values = [];
        }

        switch (location)
        {
            case ScrubberLocation.First:
                values.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                values.Add(scrubber);
                break;
        }
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch) =>
        AddScrubber(extension, Scrubber.RemoveLinesContaining(comparison, stringToMatch));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(string extension, Func<string, bool> removeLine) =>
        AddScrubber(extension, Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// No per line string is allocated for span predicates.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ScrubLines(extension, (ReadOnlySpan&lt;char&gt; line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public static void ScrubLines(string extension, LineMatch removeLine) =>
        AddScrubber(extension, Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines(string extension) =>
        AddScrubber(extension, Scrubber.RemoveEmptyLines());

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids(string extension) =>
        AddScrubber(extension, GuidMatcher.Instance);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine) =>
        AddScrubber(extension, Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// No per line string is allocated for span based replacers.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ScrubLinesWithReplace(extension, (ReadOnlySpan&lt;char&gt; line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public static void ScrubLinesWithReplace(string extension, LineReplace replaceLine) =>
        AddScrubber(extension, Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.MachineScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.UserScrubber());

    /// <summary>
    /// Replace every occurrence of <paramref name="find" /> with <paramref name="replacement" />.
    /// <paramref name="comparison" /> must be <see cref="StringComparison.Ordinal" /> or <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static void ScrubReplace(string extension, string find, string replacement, StringComparison comparison = StringComparison.Ordinal, bool requireWordBoundary = false) =>
        AddScrubber(extension, Scrubber.Replace(find, replacement, comparison, requireWordBoundary));

    /// <summary>
    /// Replace every occurrence of each Find with its Replacement.
    /// At a given position the longest matching Find wins.
    /// <paramref name="comparison" /> must be <see cref="StringComparison.Ordinal" /> or <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static void ScrubReplace(string extension, StringComparison comparison, bool requireWordBoundary, params (string Find, string Replacement)[] pairs) =>
        AddScrubber(extension, Scrubber.Replace(comparison, requireWordBoundary, pairs));

    /// <summary>
    /// Match candidate windows of text between <paramref name="minLength" /> and <paramref name="maxLength" /> characters.
    /// At each position the engine tries the longest window first.
    /// </summary>
    public static void ScrubWindow(string extension, int minLength, int maxLength, WindowMatch matcher, bool requireWordBoundary = false) =>
        AddScrubber(extension, Scrubber.Window(minLength, maxLength, matcher, requireWordBoundary));

    /// <summary>
    /// Find matches using custom search logic.
    /// <paramref name="minLength" />: segments shorter than this are skipped (null scans everything).
    /// <paramref name="maxLength" />: used for ordering only; null (unknown) runs before all known length scrubbers.
    /// </summary>
    public static void ScrubMatch(string extension, SegmentMatch matcher, int? minLength = null, int? maxLength = null) =>
        AddScrubber(extension, Scrubber.Match(matcher, minLength, maxLength));
}
