static class GuidScrubber
{
    static readonly string GuidPattern = @"(?<=[^a-zA-Z0-9])[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}(.=[^a-zA-Z0-9])";
    static readonly Regex Regex = new(GuidPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static void ReplaceGuids(StringBuilder builder)
    {
        //{173535ae-995b-4cc6-a74e-8cd4be57039c}
        if (builder.Length < 36)
        {
            return;
        }

        var value = builder.ToString();
        if (!value.Contains('-'))
        {
            return;
        }

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
                if (IsValidStartingChar(value[index - 1]))
                {
                    AppendCurrentChar();
                    continue;
                }
            }

            if (end < value.Length)
            {
                var ch = value[end];
                if (IsValidEndingChar(ch))
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

            var convert = SerializationSettings.Convert(Counter.Current, guid);
            builder.Append(convert);
            index += 35;
        }
    }

    static bool IsValidEndingChar(char ch) =>
        IsValidChar(ch) &&
        ch != '}' &&
        ch != ')';

    static bool IsValidChar(char ch) =>
        ch != ' ' &&
        ch != '\t' &&
        ch != '\n' &&
        ch != '\r';

    static bool IsValidStartingChar(char ch) =>
        IsValidChar(ch) &&
        ch != '{' &&
        ch != '(';
}