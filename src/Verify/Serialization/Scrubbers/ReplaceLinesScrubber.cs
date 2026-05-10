sealed class ReplaceLinesScrubber(LineReplace replaceLine) :
    LineScrubber
{
    public override string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context) =>
        replaceLine(line);
}
