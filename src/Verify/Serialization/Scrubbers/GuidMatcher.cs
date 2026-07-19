// The inline guid scrubbers: a fixed 36 char window over the canonical "D" format
// and a fixed 32 char window over the "N" format.
// The "D" engine anchor jumps between '-' chars at offset 8 (the first dash of the
// "D" format), so no-match text is scanned vectorized instead of per position.
// "B" and "P" occurrences are covered by the "D" window matching between their
// delimiters. "X" is not matched: its hex components are too short to reach a window.
static class GuidMatcher
{
    public static readonly Scrubber Instance = Scrubber.GatedWindow(
        36,
        36,
        Match,
        static counter => counter.ScrubGuids,
        requireWordBoundary: true,
        anchor: WindowAnchor.Char,
        anchorChar: '-',
        anchorOffset: 8);

    // No char appears at a fixed offset in the "N" format, so no anchor applies.
    // Candidate positions are filtered by the engine's word boundary check, then by
    // the first and last char prefilter, before the parse runs.
    public static readonly Scrubber NInstance = Scrubber.GatedWindow(
        32,
        32,
        MatchN,
        static counter => counter.ScrubGuids,
        requireWordBoundary: true);

    static string? Match(CharSpan window, Counter counter, IReadOnlyDictionary<string, object> context)
    {
        // Cheap prefilter: the "D" format has dashes at fixed offsets
        if (window[8] != '-' ||
            window[13] != '-' ||
            window[18] != '-' ||
            window[23] != '-')
        {
            return null;
        }

        if (!Guid.TryParseExact(window, "D", out var guid))
        {
            return null;
        }

        return counter.Convert(guid);
    }

    static string? MatchN(CharSpan window, Counter counter, IReadOnlyDictionary<string, object> context)
    {
        if (!IsHex(window[0]) ||
            !IsHex(window[31]))
        {
            return null;
        }

        if (!Guid.TryParseExact(window, "N", out var guid))
        {
            return null;
        }

        return counter.Convert(guid);
    }

    static bool IsHex(char value) =>
        value
            is >= '0' and <= '9'
            or >= 'a' and <= 'f'
            or >= 'A' and <= 'F';
}
