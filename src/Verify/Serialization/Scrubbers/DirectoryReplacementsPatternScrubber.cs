namespace VerifyTests;

sealed class DirectoryReplacementsPatternScrubber : PatternScrubber
{
    public static readonly DirectoryReplacementsPatternScrubber Instance = new();

    DirectoryReplacementsPatternScrubber()
    {
    }

    // Reads from DirectoryReplacements which is rebuilt by UseAssembly.
    public override int MinLength => DirectoryReplacements.MinLength == 0 ? int.MaxValue : DirectoryReplacements.MinLength;
    public override int MaxLength => DirectoryReplacements.MaxLength;
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

        var pairs = DirectoryReplacements.Items;
        if (pairs.Count == 0)
        {
            return false;
        }

        if (position > 0 && char.IsLetterOrDigit(source[position - 1]))
        {
            return false;
        }

        // pairs are pre-sorted desc by Find length; longest match wins.
        foreach (var pair in pairs)
        {
            var findLen = pair.Find.Length;
            if (position + findLen > source.Length)
            {
                continue;
            }

            if (!IsPathMatch(source.Slice(position, findLen), pair.Find))
            {
                continue;
            }

            var matchLen = findLen;
            var trailPos = position + findLen;
            if (trailPos < source.Length)
            {
                var trailing = source[trailPos];
                if (char.IsLetterOrDigit(trailing))
                {
                    continue;
                }

                if (trailing is '/' or '\\')
                {
                    matchLen++;
                }
            }

            matchLength = matchLen;
            replacement = pair.Replace;
            return true;
        }

        return false;
    }

    static bool IsPathMatch(CharSpan input, string find)
    {
        for (var i = 0; i < find.Length; i++)
        {
            var inputCh = input[i];
            var findCh = find[i];

            if (inputCh is '/' or '\\')
            {
                if (findCh != '/')
                {
                    return false;
                }

                continue;
            }

            if (inputCh != findCh)
            {
                return false;
            }
        }

        return true;
    }
}
