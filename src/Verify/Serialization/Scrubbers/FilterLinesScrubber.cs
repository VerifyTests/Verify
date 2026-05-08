namespace VerifyTests;

sealed class FilterLinesScrubber(Func<string, bool> removeLine) :
    LineScrubber
{
    public override string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var lineString = line.ToString();
        if (removeLine(lineString))
        {
            return null;
        }

        return lineString;
    }
}
