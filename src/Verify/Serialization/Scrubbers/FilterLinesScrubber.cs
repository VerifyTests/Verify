sealed class FilterLinesScrubber(LineFilter removeLine) :
    LineScrubber
{
    public override string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        if (removeLine(line))
        {
            return null;
        }

        return line.ToString();
    }
}
