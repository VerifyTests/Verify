static class GuidScrubber
{
    public static void ReplaceGuids(StringBuilder builder, Counter counter)
    {
        //{173535ae-995b-4cc6-a74e-8cd4be57039c}
        if (builder.Length < 36)
        {
            return;
        }

        if (!builder.Contains('-'))
        {
            return;
        }

        var value = builder.ToString();

        builder.Clear();
        for (var index = 0; index <= value.Length; index++)
        {
            void AppendCurrentChar()
            {
                builder.Append(value[index]);
            }

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

            var substring = value.Substring(index, 36);

            if (!Guid.TryParseExact(substring, "D", out var guid))
            {
                AppendCurrentChar();
                continue;
            }

            var convert = SerializationSettings.Convert(counter, guid);
            builder.Append(convert);
            index += 35;
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