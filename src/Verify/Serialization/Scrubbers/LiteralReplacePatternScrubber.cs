namespace VerifyTests;

sealed class LiteralReplacePatternScrubber : PatternScrubber
{
    readonly string find;
    readonly string replacementValue;
    readonly bool boundaryCheck;

    public LiteralReplacePatternScrubber(string find, string replacement, bool boundaryCheck = true)
    {
        if (find.Length == 0)
        {
            throw new ArgumentException("find cannot be empty", nameof(find));
        }

        this.find = find;
        replacementValue = replacement;
        this.boundaryCheck = boundaryCheck;
    }

    public override int MinLength => find.Length;
    public override int MaxLength => find.Length;
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

        if (position + find.Length > source.Length)
        {
            return false;
        }

        var slice = source.Slice(position, find.Length);
        if (!slice.SequenceEqual(find.AsSpan()))
        {
            return false;
        }

        if (boundaryCheck)
        {
            if (position > 0 && !IsValidWrapper(source[position - 1]))
            {
                return false;
            }

            var end = position + find.Length;
            if (end < source.Length && !IsValidWrapper(source[end]))
            {
                return false;
            }
        }

        matchLength = find.Length;
        replacement = replacementValue;
        return true;
    }

    static bool IsValidWrapper(char ch) =>
        !char.IsLetterOrDigit(ch);
}
