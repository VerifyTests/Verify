namespace VerifyTests;

#region ContentScrubber

/// <summary>
/// A scrubber that operates on the entire content. Content scrubbers run
/// before pattern and line scrubbers, in registration order. Use this only
/// when the transformation cannot be expressed as a position-bounded pattern
/// or a per-line filter.
/// </summary>
public abstract class ContentScrubber
{
    /// <summary>
    /// Read the full input and write the transformed content to <paramref name="output" />.
    /// </summary>
    /// <param name="input">The current content.</param>
    /// <param name="output">Empty buffer to receive the transformed content.</param>
    /// <param name="counter">Per-verification counter.</param>
    /// <param name="context">Per-verification context dictionary.</param>
    public abstract void Process(
        CharSpan input,
        StringBuilder output,
        Counter counter,
        IReadOnlyDictionary<string, object> context);
}

#endregion
