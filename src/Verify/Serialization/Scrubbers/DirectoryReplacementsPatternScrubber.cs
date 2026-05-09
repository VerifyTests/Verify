namespace VerifyTests;

sealed class DirectoryReplacementsPatternScrubber : PatternScrubber
{
    public static readonly DirectoryReplacementsPatternScrubber Instance = new();

    readonly List<DirectoryReplacements.Pair>? overrideItems;
    readonly int overrideMin;
    readonly int overrideMax;

    DirectoryReplacementsPatternScrubber()
    {
    }

    // Test seam: lets callers run the matcher against an explicit pair list
    // without going through the static DirectoryReplacements registration.
    internal DirectoryReplacementsPatternScrubber(List<DirectoryReplacements.Pair> items)
    {
        overrideItems = items;
        if (items.Count == 0)
        {
            return;
        }

        var min = int.MaxValue;
        var max = 0;
        foreach (var item in items)
        {
            if (item.Find.Length < min)
            {
                min = item.Find.Length;
            }

            if (item.Find.Length > max)
            {
                max = item.Find.Length;
            }
        }

        overrideMin = min;
        overrideMax = max + 1;
    }

    // Reads from DirectoryReplacements (rebuilt by UseAssembly) unless overridden via the test ctor.
    public override int MinLength
    {
        get
        {
            if (overrideItems != null)
            {
                return overrideItems.Count == 0 ? int.MaxValue : overrideMin;
            }

            return DirectoryReplacements.MinLength == 0 ? int.MaxValue : DirectoryReplacements.MinLength;
        }
    }

    public override int MaxLength => overrideItems != null ? overrideMax : DirectoryReplacements.MaxLength;
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

        var pairs = overrideItems ?? DirectoryReplacements.Items;
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
