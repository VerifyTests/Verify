enum ScrubberKind
{
    Replace,
    Window,
    Match,
    LineDropNeedles,
    LineDropSpan,
    LineDropString,
    LineDropEmpty,
    LineTransformSpan,
    LineTransformString,
}

// A cheap vectorized scan target for Window scrubbers: a match can only start
// where the anchor appears at the given offset from the window start, so the
// engine can jump between candidate positions instead of probing every one.
enum WindowAnchor
{
    None,
    Char,
    Digit,
}
