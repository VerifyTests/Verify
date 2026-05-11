sealed class FilterLinesScrubber(LineFilter removeLine) :
    LineScrubber
{
    public override bool Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out CharSpan replacement)
    {
        replacement = line;
        return !removeLine(line);
    }
}
