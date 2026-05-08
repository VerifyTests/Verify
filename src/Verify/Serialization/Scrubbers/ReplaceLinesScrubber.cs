namespace VerifyTests;

sealed class ReplaceLinesScrubber(Func<string, string?> replaceLine) :
    LineScrubber
{
    public override string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context) =>
        replaceLine(line.ToString());
}
