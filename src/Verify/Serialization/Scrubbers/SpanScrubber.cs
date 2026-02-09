namespace VerifyTests;

public readonly record struct SpanScrubResult(bool Matched, string? Replacement);

public delegate SpanScrubResult SpanScrubHandler(CharSpan span, Counter counter);

public class SpanScrubber(int? minLength, int? maxLength, SpanScrubHandler tryConvert)
{
    public int? MinLength { get; } = minLength;
    public int? MaxLength { get; } = maxLength;
    public SpanScrubHandler TryConvert { get; } = tryConvert;
}
