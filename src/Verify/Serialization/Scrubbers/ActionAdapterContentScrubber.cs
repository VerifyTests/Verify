namespace VerifyTests;

/// <summary>
/// Adapts a legacy <c>Action&lt;StringBuilder, Counter, IReadOnlyDictionary&lt;string, object&gt;&gt;</c>
/// scrubber to the new <see cref="ContentScrubber" /> contract. Used by the
/// obsoleted AddScrubber Action overloads.
/// </summary>
sealed class ActionAdapterContentScrubber(Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> action) :
    ContentScrubber
{
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
