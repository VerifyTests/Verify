namespace VerifyTests;

/// <summary>
/// Attempts to match a candidate window of text.
/// The window length is between the minLength and maxLength of the owning <see cref="Scrubber" /> and never contains a line break.
/// Return the replacement text, or null when the window is not a match.
/// </summary>
public delegate string? WindowMatch(
    CharSpan window,
    Counter counter,
    IReadOnlyDictionary<string, object> context);

/// <summary>
/// Attempts to find the next match within a segment of text.
/// The segment may contain line breaks, but the matched range must not.
/// Return true and set <paramref name="index" />, <paramref name="length" />, and <paramref name="replacement" /> when a match is found.
/// </summary>
public delegate bool SegmentMatch(
    CharSpan segment,
    Counter counter,
    IReadOnlyDictionary<string, object> context,
    out int index,
    out int length,
    out string? replacement);

/// <summary>
/// Determines if a line matches. The line excludes its terminator.
/// </summary>
public delegate bool LineMatch(CharSpan line);

/// <summary>
/// Produces a <see cref="LineResult" /> for a line. The line excludes its terminator.
/// </summary>
public delegate LineResult LineReplace(CharSpan line);
