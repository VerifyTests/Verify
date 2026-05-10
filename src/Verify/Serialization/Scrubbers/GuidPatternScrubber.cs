sealed class GuidPatternScrubber : PatternScrubber
{
    public static readonly GuidPatternScrubber Instance = new();

    GuidPatternScrubber()
    {
    }

    public override int MinLength => 36;
    public override int MaxLength => 36;
    public override bool SingleLine => true;

    public override bool TryMatch(
        CharSpan source,
        int position,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchLength,
        [NotNullWhen(true)] out string? replacement)
    {
        matchLength = 0;
        replacement = null;

        if (!counter.ScrubGuids)
        {
            return false;
        }

        if (position + 36 > source.Length)
        {
            return false;
        }

        if (position > 0 && IsInvalidStartingChar(source[position - 1]))
        {
            return false;
        }

        var end = position + 36;
        if (end < source.Length && IsInvalidEndingChar(source[end]))
        {
            return false;
        }

        var slice = source.Slice(position, 36);
        if (!Guid.TryParseExact(slice, "D", out var guid))
        {
            return false;
        }

        replacement = counter.Convert(guid);
        matchLength = 36;
        return true;
    }

    static bool IsInvalidEndingChar(char ch) =>
        IsInvalidChar(ch) &&
        ch != '}' &&
        ch != ')';

    static bool IsInvalidChar(char ch) =>
        char.IsLetter(ch) ||
        char.IsNumber(ch);

    static bool IsInvalidStartingChar(char ch) =>
        IsInvalidChar(ch) &&
        ch != '{' &&
        ch != '(';
}
