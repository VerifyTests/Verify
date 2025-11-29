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

        CrossChunkMatcher.ReplaceAll(
            builder,
            maxLength: 36,
            context: (Builder: builder, Counter: counter),
            matcher: static (content, absolutePosition, context) =>
            {
                // Need at least 36 characters for a GUID
                if (content.Length < 36)
                {
                    return MatchResult.NoMatch();
                }

                // Validate start boundary (check character before the potential GUID)
                if (absolutePosition > 0 &&
                    IsInvalidStartingChar(context.Builder[absolutePosition - 1]))
                {
                    return MatchResult.NoMatch();
                }

                // Validate end boundary (check character after the potential GUID)
                var endPosition = absolutePosition + 36;
                if (endPosition < context.Builder.Length &&
                    IsInvalidEndingChar(context.Builder[endPosition]))
                {
                    return MatchResult.NoMatch();
                }

                // Try to parse as GUID
                var slice = content.Slice(0, 36);
                if (!Guid.TryParseExact(slice, "D", out var guid))
                {
                    return MatchResult.NoMatch();
                }

                // Convert and return match
                var converted = context.Counter.Convert(guid);
                return MatchResult.Match(36, converted);
            });
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