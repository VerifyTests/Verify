namespace VerifyTests;

/// <summary>
/// A <see cref="ContentScrubber" /> that runs a delegate against the assembled
/// content. Intended as a migration aid for callers moving away from the legacy
/// <c>AddScrubber(Action&lt;StringBuilder&gt;)</c> overloads — the action receives
/// the output buffer pre-populated with the input, and is free to mutate it in
/// place.
/// </summary>
/// <remarks>
/// Prefer <see cref="PatternScrubber" /> or <see cref="LineScrubber" /> when the
/// transformation can be expressed as a bounded pattern match or a per-line
/// filter. Those tiers let the engine skip work that this delegate-style scrubber
/// always pays for.
/// </remarks>
public sealed class LambdaContentScrubber(Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> action)
    : ContentScrubber
{
    public LambdaContentScrubber(Action<StringBuilder> action) :
        this((builder, _, _) => action(builder))
    {
    }

    public LambdaContentScrubber(Action<StringBuilder, Counter> action) :
        this((builder, counter, _) => action(builder, counter))
    {
    }

    public override void Process(
        CharSpan input,
        StringBuilder output,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        output.Append(input);
        action(output, counter, context);
    }
}
