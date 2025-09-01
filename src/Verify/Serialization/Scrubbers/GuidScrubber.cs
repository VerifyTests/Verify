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

        if (builder.Count('-') < 4)
        {
            return;
        }

        var value = builder.ToString().AsSpan();

        var builderIndex = 0;
        for (var index = 0; index <= value.Length; index++)
        {
            var end = index + 36;
            if (end > value.Length)
            {
                return;
            }

            if ((index == 0 || !IsInvalidStartingChar(value[index - 1])) &&
                (end == value.Length || !IsInvalidEndingChar(value[end])))
            {
                var slice = value.Slice(index, 36);
                if (!slice.ContainsNewline() &&
                    Guid.TryParseExact(slice, "D", out var guid))
                {
                    var convert = counter.Convert(guid);
                    builder.Overwrite(convert, builderIndex, 36);
                    builderIndex += convert.Length;
                    index += 35;

                    continue;
                }
            }

            builderIndex++;
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
}