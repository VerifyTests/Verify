namespace VerifyTests;

/// <summary>
/// A single emit produced by the pattern walker. Either a passthrough range
/// referencing the source buffer, or a replacement string emitted by a scrubber.
/// </summary>
readonly struct ScrubberChunk
{
    public readonly int SourceStart;
    public readonly int SourceLength;
    public readonly string? Replacement;

    ScrubberChunk(int sourceStart, int sourceLength, string? replacement)
    {
        SourceStart = sourceStart;
        SourceLength = sourceLength;
        Replacement = replacement;
    }

    public static ScrubberChunk Passthrough(int start, int length) =>
        new(start, length, null);

    public static ScrubberChunk Replace(string replacement) =>
        new(-1, replacement.Length, replacement);

    public int Length => Replacement?.Length ?? SourceLength;
}
