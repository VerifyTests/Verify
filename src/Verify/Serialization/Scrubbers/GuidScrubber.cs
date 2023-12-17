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

        var value = builder.ToString();

        builder.Clear();
        for (var index = 0; index <= value.Length; index++)
        {
            void AppendCurrentChar() => builder.Append(value[index]);

            var end = index + 36;
            if (end > value.Length)
            {
                var remaining = value[index..];
                builder.Append(remaining);
                return;
            }

            if (index > 0)
            {
                if (IsInvalidStartingChar(value[index - 1]))
                {
                    AppendCurrentChar();
                    continue;
                }
            }

            if (end < value.Length)
            {
                var ch = value[end];
                if (IsInvalidEndingChar(ch))
                {
                    AppendCurrentChar();
                    continue;
                }
            }

            var tryParseExact = TryParse(value, index, out var guid);
            if (!tryParseExact)
            {
                AppendCurrentChar();
                continue;
            }

            var convert = SerializationSettings.Convert(counter, guid);
            builder.Append(convert);
            index += 35;
        }
    }

    static bool TryParse(string value, int index, out Guid guid)
    {
#if NET6_0_OR_GREATER
        var substring = value.AsSpan().Slice(index, 36);
#else
        var substring = value.Substring(index, 36);
#endif
        return Guid.TryParseExact(substring, "D", out guid);
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