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

        builder.Clear();
        for (var index = 0; index <= value.Length; index++)
        {
            var end = index + 36;
            if (end > value.Length)
            {
                var remaining = value[index..];
                builder.Append(remaining);
                return;
            }

            if ((index == 0 || !IsInvalidStartingChar(value[index - 1])) &&
                (end == value.Length || !IsInvalidEndingChar(value[end])))
            {
                if (TryParse(value, index, out var guid))
                {
                    var convert = SerializationSettings.Convert(counter, guid);
                    builder.Append(convert);
                    index += 35;
                    continue;
                }
            }

            builder.Append(value[index]);
        }
    }

    static bool TryParse(CharSpan value, int index, out Guid guid)
    {
        var substring = value.Slice(index, 36);
#if NET6_0_OR_GREATER
        return Guid.TryParseExact(substring, "D", out guid);
#else
        return Guid.TryParseExact(substring.ToString(), "D", out guid);
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