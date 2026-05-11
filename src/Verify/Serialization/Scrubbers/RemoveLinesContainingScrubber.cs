namespace VerifyTests;

sealed class RemoveLinesContainingScrubber(StringComparison comparison, string[] stringToMatch) :
    LineScrubber
{
    public override bool Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out CharSpan replacement)
    {
        replacement = line;
        foreach (var toMatch in stringToMatch)
        {
            if (line.Contains(toMatch.AsSpan(), comparison))
            {
                return false;
            }
        }

        return true;
    }
}
