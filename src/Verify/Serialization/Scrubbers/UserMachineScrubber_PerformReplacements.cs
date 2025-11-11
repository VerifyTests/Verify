static partial class UserMachineScrubber
{
    static bool IsValidWrapper(char ch) =>
        ch is
            ' ' or
            '\t' or
            '\n' or
            '\r';

    public static void PerformReplacements(StringBuilder builder, string find, string replace)
    {
        if (builder.Length < find.Length)
        {
            return;
        }

        var matches = FindMatches(builder, find);

        // Sort by position descending
        var orderByDescending = matches.OrderByDescending(_ => _);

        // Apply matches
        foreach (var match in orderByDescending)
        {
            builder.Overwrite(replace, match, find.Length);
        }
    }

    static IEnumerable<int> FindMatches(StringBuilder builder, string find)
    {
        var absolutePosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            if (chunk.Length < find.Length)
            {
                absolutePosition += chunk.Length;
                continue;
            }

            var chunkIndex = 0;
            while (true)
            {
                var value = chunk.Span;
                chunkIndex = value[chunkIndex..].IndexOf(find);
                if (chunkIndex == -1)
                {
                    break;
                }

                var end = chunkIndex + find.Length;
                if ((chunkIndex != 0 && !IsValidWrapper(value[chunkIndex - 1])) ||
                    (end != value.Length && !IsValidWrapper(value[end])))
                {
                    chunkIndex++;
                    continue;
                }

                yield return absolutePosition + chunkIndex;
                chunkIndex += find.Length;
            }

            absolutePosition += chunk.Length;
        }
    }
}