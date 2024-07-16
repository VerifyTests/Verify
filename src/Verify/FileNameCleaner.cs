static class FileNameCleaner
{
    static char[] invalidFileNameChars =
    [
        '"',
        '\\',
        '<',
        '>',
        '|',
        '\u0000',
        '\u0001',
        '\u0002',
        '\u0003',
        '\u0004',
        '\u0005',
        '\u0006',
        '\u0007',
        '\b',
        '\t',
        '\n',
        '\u000b',
        '\f',
        '\r',
        '\u000e',
        '\u000f',
        '\u0010',
        '\u0011',
        '\u0012',
        '\u0013',
        '\u0014',
        '\u0015',
        '\u0016',
        '\u0017',
        '\u0018',
        '\u0019',
        '\u001a',
        '\u001b',
        '\u001c',
        '\u001d',
        '\u001e',
        '\u001f',
        ':',
        '*',
        '?',
        '/'
    ];

#if NET8_0_OR_GREATER
    static SearchValues<char> invalidFileNameSearchValues = SearchValues.Create(invalidFileNameChars);
#endif

    public static string ReplaceInvalidFileNameChars(this string value)
    {
        var span = value.AsSpan();

        var index = IndexOfInvalidChar(span);

        if (index == -1)
        {
            return value;
        }

        var chars = value.ToCharArray();
        span[..index]
            .CopyTo(chars);
        chars[index] = '-';
        index++;
        for (; index < chars.Length; index++)
        {
            if (IsInvalid(chars[index]))
            {
                chars[index] = '-';
            }
        }

        return new(chars);
    }

    static int IndexOfInvalidChar(CharSpan span) =>
#if NET8_0_OR_GREATER
        span.IndexOfAny(invalidFileNameSearchValues);
#else
        span.IndexOfAny(invalidFileNameChars.AsSpan());
#endif


    static bool IsInvalid(char ch) =>
#if NET8_0_OR_GREATER
        invalidFileNameSearchValues.Contains(ch);
#else
        invalidFileNameChars.Contains(ch);
#endif

    public static void AppendValid(StringBuilder builder, string value)
    {
        var span = value.AsSpan();
        var index = IndexOfInvalidChar(span);

        if (index == -1)
        {
            builder.Append(value);
            return;
        }

        foreach (var ch in value)
        {
            if (IsInvalid(ch))
            {
                builder.Append('-');
            }
            else
            {
                builder.Append(ch);
            }
        }
    }
}