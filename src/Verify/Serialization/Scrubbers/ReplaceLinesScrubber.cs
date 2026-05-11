sealed class ReplaceLinesScrubber(LineReplace replaceLine) :
    LineScrubber
{
    public override bool Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out CharSpan replacement) =>
        replaceLine(line, out replacement);
}
