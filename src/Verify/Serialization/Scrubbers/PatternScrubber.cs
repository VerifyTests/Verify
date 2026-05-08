namespace VerifyTests;

#region PatternScrubber

/// <summary>
/// A scrubber that matches and replaces patterns at known positions in the input.
/// The engine drives a single-pass walk; <see cref="TryMatch" /> is invoked at each
/// candidate position. Once a match is committed at a position, no other scrubber
/// processes that range.
/// </summary>
public abstract class PatternScrubber
{
    /// <summary>
    /// Minimum length of input this scrubber could match. The engine skips
    /// invocations where <c>source.Length - position</c> is shorter than this.
    /// </summary>
    public abstract int MinLength { get; }

    /// <summary>
    /// Maximum length this scrubber will consume in a single match. Used to order
    /// scrubbers (longest-first wins overlapping matches by default) and to bound
    /// the candidate window. The scrubber must not return a <c>matchLength</c>
    /// greater than this value.
    /// </summary>
    public abstract int MaxLength { get; }

    /// <summary>
    /// True when matches cannot span newlines. Enables the engine's per-line
    /// fast-path (skip lines shorter than <see cref="MinLength" />, bound match
    /// attempts to line boundaries).
    /// </summary>
    public abstract bool SingleLine { get; }

    /// <summary>
    /// Try to match starting at <paramref name="position" /> within <paramref name="source" />.
    /// The scrubber may peek at preceding (<c>source[position - 1]</c>) and trailing
    /// (<c>source[position + matchLength]</c>) characters for boundary checks, but
    /// must not consume more than <see cref="MaxLength" /> chars.
    /// </summary>
    /// <param name="source">The full source span being scanned.</param>
    /// <param name="position">The candidate match position (0-based).</param>
    /// <param name="counter">Per-verification counter for deduplicating substituted values.</param>
    /// <param name="context">Per-verification context dictionary.</param>
    /// <param name="matchLength">
    /// When matched, the number of chars consumed from <paramref name="position" />.
    /// Must be <c>&gt;= 1</c> and <c>&lt;= MaxLength</c>.
    /// </param>
    /// <param name="replacement">When matched, the string to emit in place of the consumed chars.</param>
    /// <returns><c>true</c> if a match was found and consumed.</returns>
    public abstract bool TryMatch(
        CharSpan source,
        int position,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchLength,
        [NotNullWhen(true)] out string? replacement);
}

#endregion
