namespace VerifyTests;

sealed class RemoveEmptyLinesScrubber : LineScrubber
{
    public static readonly RemoveEmptyLinesScrubber Instance = new();

    RemoveEmptyLinesScrubber()
    {
    }

    public override bool Process(
        CharSpan line,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out CharSpan replacement)
    {
        replacement = line;
        return !line.IsWhiteSpace();
    }
}
