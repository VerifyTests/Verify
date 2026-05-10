sealed class ReplaceLinesScrubber(LineReplace replaceLine) :
    LineScrubber
{
    public override bool Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out string? replacement)
    {
        replacement = replaceLine(line);
        return replacement is not null;
    }
}
