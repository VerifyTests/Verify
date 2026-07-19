// The inline guid scrubber: a fixed 36 char window over the canonical "D" format.
// The engine anchor jumps between '-' chars at offset 8 (the first dash of the "D"
// format), so no-match text is scanned vectorized instead of per position.
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
}
