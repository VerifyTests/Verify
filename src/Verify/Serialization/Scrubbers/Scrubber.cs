/// <summary>
/// Defines a scrubbing operation executed by the span based scrub engine.
/// </summary>
/// <remarks>
/// <para>
/// Semantics shared by all scrubbers created from this type:
/// </para>
/// <list type="bullet">
/// <item><b>Quarantine</b>: text produced by a replacement is never re-examined by other <see cref="Scrubber" />s.
/// Legacy <c>AddScrubber(Action&lt;StringBuilder&gt;)</c> scrubbers run afterwards and can still modify it.</item>
/// <item><b>Ordering</b>: line removals run first, then line transforms (registration order), then inline
/// scrubbers: unknown max length first, then descending max length, ties broken by registration level
/// (instance, extension mapped instance, extension mapped global, global) then registration order.
/// Path replacements (<c>{ProjectDirectory}</c> etc) always run last.</item>
/// <item><b>Length rules</b>: text shorter than a scrubber's minimum length is skipped without invoking the scrubber.</item>
/// <item><b>Single line rule</b>: a match may never contain a line break. Finds may not contain <c>\n</c> or <c>\r</c>.</item>
/// <item><b>Word boundary</b>: when required, a match is rejected if the character on either side is a letter or digit.</item>
/// </list>
/// </remarks>
sealed class Scrubber
{
    internal ScrubberKind Kind { get; }
    internal int MinLength { get; }
    internal int? MaxLength { get; }
    internal StringComparison Comparison { get; }
    internal bool RequireWordBoundary { get; }
    internal WindowAnchor Anchor { get; }
    internal char AnchorChar { get; }
    internal int AnchorOffset { get; }
    internal (string Find, string Replacement)[]? Pairs { get; }
    internal string[]? Needles { get; }
    internal WindowMatch? WindowMatcher { get; }
    internal SegmentMatch? SegmentMatcher { get; }
    internal LineMatch? LineMatcher { get; }
    internal Func<string, bool>? LineStringMatcher { get; }
    internal LineReplace? LineReplacer { get; }
    internal Func<string, string?>? LineStringReplacer { get; }

    // When set and it returns false, the engine skips this scrubber for the whole
    // scrub rather than invoking it per candidate
    internal Func<Counter, bool>? Gate { get; }

    // When set, the instance actually run is resolved at scrub time. Used by the
    // built-in date scrubbers, whose parse culture, window bounds, and anchor all
    // depend on the culture in effect when the scrub runs.
    Func<Scrubber>? resolver;

    internal Scrubber Resolve() =>
        resolver?.Invoke() ?? this;

    Scrubber(
        ScrubberKind kind,
        int minLength = 0,
        int? maxLength = null,
        StringComparison comparison = StringComparison.Ordinal,
        bool requireWordBoundary = false,
        (string Find, string Replacement)[]? pairs = null,
        string[]? needles = null,
        WindowMatch? windowMatcher = null,
        SegmentMatch? segmentMatcher = null,
        LineMatch? lineMatcher = null,
        Func<string, bool>? lineStringMatcher = null,
        LineReplace? lineReplacer = null,
        Func<string, string?>? lineStringReplacer = null,
        WindowAnchor anchor = WindowAnchor.None,
        char anchorChar = '\0',
        int anchorOffset = 0,
        Func<Counter, bool>? gate = null,
        Func<Scrubber>? resolver = null)
    {
        Kind = kind;
        MinLength = minLength;
        MaxLength = maxLength;
        Comparison = comparison;
        RequireWordBoundary = requireWordBoundary;
        Pairs = pairs;
        Needles = needles;
        WindowMatcher = windowMatcher;
        SegmentMatcher = segmentMatcher;
        LineMatcher = lineMatcher;
        LineStringMatcher = lineStringMatcher;
        LineReplacer = lineReplacer;
        LineStringReplacer = lineStringReplacer;
        Anchor = anchor;
        AnchorChar = anchorChar;
        AnchorOffset = anchorOffset;
        Gate = gate;
        this.resolver = resolver;
    }

    internal bool IsLineDrop =>
        Kind is ScrubberKind.LineDropNeedles
            or ScrubberKind.LineDropSpan
            or ScrubberKind.LineDropString
            or ScrubberKind.LineDropEmpty;

    internal bool IsLineTransform =>
        Kind is ScrubberKind.LineTransformSpan
            or ScrubberKind.LineTransformString;

    /// <summary>
    /// Replace every occurrence of <paramref name="find" /> with <paramref name="replacement" />.
    /// <paramref name="comparison" /> must be <see cref="StringComparison.Ordinal" /> or <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static Scrubber Replace(
        string find,
        string replacement,
        StringComparison comparison = StringComparison.Ordinal,
        bool requireWordBoundary = false)
    {
        ValidateFind(find);
        Ensure.NotNull(replacement);
        ValidateOrdinal(comparison);
        return new(
            ScrubberKind.Replace,
            minLength: find.Length,
            maxLength: find.Length,
            comparison: comparison,
            requireWordBoundary: requireWordBoundary,
            pairs: [(find, NormalizeNewlines(replacement))]);
    }

    /// <summary>
    /// Replace every occurrence of each Find with its Replacement.
    /// At a given position the longest matching Find wins.
    /// <paramref name="comparison" /> must be <see cref="StringComparison.Ordinal" /> or <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static Scrubber Replace(
        StringComparison comparison,
        bool requireWordBoundary,
        params (string Find, string Replacement)[] pairs)
    {
        Ensure.NotNullOrEmpty(pairs);
        ValidateOrdinal(comparison);
        var ordered = new (string Find, string Replacement)[pairs.Length];
        for (var index = 0; index < pairs.Length; index++)
        {
            var (find, replacement) = pairs[index];
            ValidateFind(find);
            Ensure.NotNull(replacement);
            ordered[index] = (find, NormalizeNewlines(replacement));
        }

        // Longest first so the most specific Find wins at any given position
        Array.Sort(ordered, (left, right) => right.Find.Length.CompareTo(left.Find.Length));
        return new(
            ScrubberKind.Replace,
            minLength: ordered[^1].Find.Length,
            maxLength: ordered[0].Find.Length,
            comparison: comparison,
            requireWordBoundary: requireWordBoundary,
            pairs: ordered);
    }

    /// <summary>
    /// Match candidate windows of text between <paramref name="minLength" /> and <paramref name="maxLength" /> characters.
    /// At each position the engine tries the longest window first.
    /// </summary>
    public static Scrubber Window(
        int minLength,
        int maxLength,
        WindowMatch matcher,
        bool requireWordBoundary = false)
    {
        Ensure.NotNull(matcher);
        ValidateWindowLengths(minLength, maxLength);

        return new(
            ScrubberKind.Window,
            minLength: minLength,
            maxLength: maxLength,
            requireWordBoundary: requireWordBoundary,
            windowMatcher: matcher);
    }

    static void ValidateWindowLengths(int minLength, int maxLength)
    {
        if (minLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minLength), minLength, "minLength must be at least 1.");
        }

        if (maxLength < minLength)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "maxLength must be greater than or equal to minLength.");
        }
    }

    // A Window scrubber used by the built-in guid and date scrubbers.
    // The gate is evaluated once per scrub, so a disabled built-in costs a single
    // check instead of a full scan.
    // When an anchor is supplied, matches can only start where it appears at
    // anchorOffset from the window start, so no-match scans skip between candidate
    // positions.
    internal static Scrubber GatedWindow(
        int minLength,
        int maxLength,
        WindowMatch matcher,
        Func<Counter, bool> gate,
        bool requireWordBoundary = false,
        WindowAnchor anchor = WindowAnchor.None,
        char anchorChar = '\0',
        int anchorOffset = 0,
        Func<Scrubber>? resolver = null)
    {
        ValidateWindowLengths(minLength, maxLength);

        return new(
            ScrubberKind.Window,
            minLength: minLength,
            maxLength: maxLength,
            requireWordBoundary: requireWordBoundary,
            windowMatcher: matcher,
            anchor: anchor,
            anchorChar: anchorChar,
            anchorOffset: anchorOffset,
            gate: gate,
            resolver: resolver);
    }

    /// <summary>
    /// Find matches using custom search logic.
    /// <paramref name="minLength" />: segments shorter than this are skipped (null scans everything).
    /// <paramref name="maxLength" />: used for ordering only; null (unknown) runs before all known length scrubbers.
    /// </summary>
    public static Scrubber Match(
        SegmentMatch matcher,
        int? minLength = null,
        int? maxLength = null)
    {
        Ensure.NotNull(matcher);
        if (minLength is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minLength), minLength, "minLength must be at least 1.");
        }

        if (maxLength < minLength)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "maxLength must be greater than or equal to minLength.");
        }

        return new(
            ScrubberKind.Match,
            minLength: minLength ?? 0,
            maxLength: maxLength,
            segmentMatcher: matcher);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="needles" />.
    /// </summary>
    public static Scrubber RemoveLinesContaining(StringComparison comparison, params string[] needles)
    {
        Ensure.NotNullOrEmpty(needles);
        var copy = new string[needles.Length];
        var minLength = int.MaxValue;
        for (var index = 0; index < needles.Length; index++)
        {
            var needle = needles[index];
            ValidateFind(needle);
            copy[index] = needle;
            minLength = Math.Min(minLength, needle.Length);
        }

        return new(
            ScrubberKind.LineDropNeedles,
            // A linguistic comparison can match a run shorter than the needle, so
            // a line shorter than the needle cannot be skipped unread
            minLength: IsOrdinal(comparison) ? minLength : 0,
            comparison: comparison,
            needles: copy);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="needles" />, using <see cref="StringComparison.OrdinalIgnoreCase" />.
    /// </summary>
    public static Scrubber RemoveLinesContaining(params string[] needles) =>
        RemoveLinesContaining(StringComparison.OrdinalIgnoreCase, needles);

    /// <summary>
    /// Remove any lines matching <paramref name="shouldRemove" />.
    /// No per line string is allocated for span predicates.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>RemoveLines((ReadOnlySpan&lt;char&gt; line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public static Scrubber RemoveLines(LineMatch shouldRemove)
    {
        Ensure.NotNull(shouldRemove);
        return new(ScrubberKind.LineDropSpan, lineMatcher: shouldRemove);
    }

    /// <summary>
    /// Remove any lines matching <paramref name="shouldRemove" />.
    /// </summary>
    public static Scrubber RemoveLines(Func<string, bool> shouldRemove)
    {
        Ensure.NotNull(shouldRemove);
        return new(ScrubberKind.LineDropString, lineStringMatcher: shouldRemove);
    }

    /// <summary>
    /// Process each line via <paramref name="replace" />.
    /// No per line string is allocated for span based replacers.
    /// Use an explicitly typed lambda parameter to select this overload,
    /// e.g. <c>ReplaceLines((ReadOnlySpan&lt;char&gt; line) => ...)</c>.
    /// </summary>
    [OverloadResolutionPriority(-1)]
    public static Scrubber ReplaceLines(LineReplace replace)
    {
        Ensure.NotNull(replace);
        return new(ScrubberKind.LineTransformSpan, lineReplacer: replace);
    }

    /// <summary>
    /// Process each line via <paramref name="replace" />.
    /// <paramref name="replace" /> can return the input to keep the line, a different string to replace it, or null to remove it.
    /// </summary>
    public static Scrubber ReplaceLines(Func<string, string?> replace)
    {
        Ensure.NotNull(replace);
        return new(ScrubberKind.LineTransformString, lineStringReplacer: replace);
    }

    /// <summary>
    /// Remove any lines containing only whitespace.
    /// </summary>
    public static Scrubber RemoveEmptyLines() =>
        new(ScrubberKind.LineDropEmpty);

    internal static string NormalizeNewlines(string value)
    {
        if (!value.Contains('\r'))
        {
            return value;
        }

        return value
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');
    }

    // A replacement splices out exactly Find.Length chars, and the engine skips
    // text shorter than Find. Both only hold for ordinal comparisons: a linguistic
    // comparison can match a run of a different length than Find (for example an
    // ignorable soft hyphen, or 'ss' matching 'ß'), which would splice the wrong
    // range.
    static void ValidateOrdinal(StringComparison comparison)
    {
        if (comparison is StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase)
        {
            return;
        }

        throw new ArgumentException(
            $"Only Ordinal and OrdinalIgnoreCase are supported, but was {comparison}. A linguistic comparison can match a different number of characters than the find, so the match cannot be replaced reliably.",
            nameof(comparison));
    }

    // True when a match is guaranteed to be the same length as the needle, so
    // text shorter than the needle can be skipped without comparing
    static bool IsOrdinal(StringComparison comparison) =>
        comparison is StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase;

    static void ValidateFind(string find)
    {
        Ensure.NotNullOrEmpty(find);
        if (find.Contains('\n') ||
            find.Contains('\r'))
        {
            throw new ArgumentException($"Find must not contain line breaks: {find}", nameof(find));
        }
    }
}
