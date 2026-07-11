// The inline guid scrubber: a fixed 36 char window over the canonical "D" format,
// with a dash prefilter so the parse is only attempted at plausible positions.
static class GuidMatcher
{
    public static readonly Scrubber Instance = Scrubber.Window(36, 36, Match, requireWordBoundary: true);

    static string? Match(CharSpan window, Counter counter, IReadOnlyDictionary<string, object> context)
    {
        if (!counter.ScrubGuids)
        {
            return null;
        }

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
