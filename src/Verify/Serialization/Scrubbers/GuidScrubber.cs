static class GuidScrubber
{
    public static void ReplaceGuids(StringBuilder builder, Counter counter)
    {
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
                if (!slice.ContainsNewline() && TryParse(slice, out var guid))
                {
                    var convert = SerializationSettings.Convert(counter, guid);
                    builder.Overwrite(convert, builderIndex, 36);
                    builderIndex += convert.Length;
                    index += 35;

                    continue;
                }
            }

            builderIndex++;
        }
    }

    static bool TryParse(CharSpan slice, out Guid guid)
    {
#if NET6_0_OR_GREATER
        return Guid.TryParseExact(slice, "D", out guid);
#else
        return Guid.TryParseExact(slice.ToString(), "D", out guid);
#endif
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