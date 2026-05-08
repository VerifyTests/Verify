namespace VerifyTests;

sealed class RemoveEmptyLinesScrubber : LineScrubber
{
    public static readonly RemoveEmptyLinesScrubber Instance = new();

    RemoveEmptyLinesScrubber()
    {
    }

    public override string? Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        if (line.IsWhiteSpace())
        {
            return null;
        }

        return line.ToString();
    }
}
