namespace VerifyTests;

#region LineScrubber

/// <summary>
/// A scrubber that processes one line at a time. Line scrubbers run after
/// pattern scrubbers, in registration order. Multiple line scrubbers chain:
/// each receives the line as transformed by the previous one.
/// </summary>
public abstract class LineScrubber
{
    /// <summary>
    /// Process a single line.
    /// </summary>
    /// <param name="line">The line content (no trailing newline).</param>
    /// <param name="counter">Per-verification counter.</param>
    /// <param name="context">Per-verification context dictionary.</param>
    /// <param name="replacement">
    /// When the return value is <c>true</c>, the content to emit. Assign
    /// <paramref name="line" /> itself to keep the line unchanged with zero allocation.
    /// The span must remain valid until the call returns; backing a span with a
    /// per-call string (or string literal) is the normal pattern for replacements.
    /// </param>
    /// <returns>
    /// <c>false</c> to drop the line entirely; <c>true</c> to keep it.
    /// </returns>
    public abstract bool Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out CharSpan replacement);
}

#endregion
