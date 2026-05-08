namespace VerifyTests;

sealed class RemoveLinesContainingScrubber(StringComparison comparison, string[] stringToMatch) :
    LineScrubber
{
    public override string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var lineString = line.ToString();
        foreach (var toMatch in stringToMatch)
        {
            if (lineString.Contains(toMatch, comparison))
            {
                return null;
            }
        }

        return lineString;
    }
}
