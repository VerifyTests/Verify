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
    /// <returns>
    /// <c>null</c> to drop the line entirely, or the (possibly modified) line
    /// content to keep. Return the input span as a string to keep unchanged.
    /// </returns>
    public abstract string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context);
}

#endregion
