static class GuidScrubber
{
    public static void ReplaceGuids(StringBuilder builder, Counter counter)
    {
        if (!counter.ScrubGuids)
        {
            return;
        }

        //{173535ae-995b-4cc6-a74e-8cd4be57039c}
        if (builder.Length < 36)
        {
            return;
        }

        var matches = FindMatches(builder, counter);

        // Sort by position descending
        var orderByDescending = matches.OrderByDescending(_ => _.Index);

        // Apply matches
        foreach (var match in orderByDescending)
        {
            builder.Overwrite(match.Value, match.Index, 36);
        }
    }

    static IEnumerable<Match> FindMatches(StringBuilder builder, Counter counter)
    {
        var absolutePosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            if (chunk.Length < 36)
            {
                continue;
            }

            for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
            {
                var end = chunkIndex + 36;
                if (end > chunk.Length)
                {
                    break;
                }

                var value = chunk.Span;
                if ((chunkIndex != 0 && IsInvalidStartingChar(value[chunkIndex - 1])) ||
                    (end != value.Length && IsInvalidEndingChar(value[end])))
                {
                    continue;
                }
                var slice = value.Slice(chunkIndex, 36);

                if (slice.ContainsNewline() ||
                    !Guid.TryParseExact(slice, "D", out var guid))
                {
                    continue;
                }

                var convert = counter.Convert(guid);
                var startReplaceIndex = absolutePosition + chunkIndex;
                yield return new(startReplaceIndex, convert);
                chunkIndex += 35;
            }

            absolutePosition += chunk.Length;
        }
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

    readonly struct Match(int index, string value)
    {
        public readonly int Index = index;
        public readonly string Value = value;
    }
}