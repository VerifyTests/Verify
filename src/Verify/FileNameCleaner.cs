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

    static char[] invalidPathChars =
    [
        '"',
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
        '?'
    ];

#if NET8_0_OR_GREATER
    static SearchValues<char> invalidFileNameSearchValues = SearchValues.Create(invalidFileNameChars);
    static SearchValues<char> invalidPathSearchValues = SearchValues.Create(invalidPathChars);
#endif

    static int IndexOfInvalidFileNameChar(CharSpan span) =>
#if NET8_0_OR_GREATER
        span.IndexOfAny(invalidFileNameSearchValues);
#else
        span.IndexOfAny(invalidFileNameChars.AsSpan());
#endif
    static int IndexOfInvalidPathChar(CharSpan span) =>
#if NET8_0_OR_GREATER
        span.IndexOfAny(invalidPathSearchValues);
#else
        span.IndexOfAny(invalidPathChars.AsSpan());
#endif

    static bool IsFileNameInvalid(char ch) =>
#if NET8_0_OR_GREATER
        invalidFileNameSearchValues.Contains(ch);
#else
        invalidFileNameChars.Contains(ch);
#endif

    static bool IsPathInvalid(char ch) =>
#if NET8_0_OR_GREATER
        invalidPathSearchValues.Contains(ch);
#else
        invalidPathChars.Contains(ch);
#endif

    public static void AppendValid(StringBuilder builder, string value)
    {
        var span = value.AsSpan();
        var index = IndexOfInvalidFileNameChar(span);

        if (index == -1)
        {
            builder.Append(value);
            return;
        }

        foreach (var ch in value)
        {
            if (IsFileNameInvalid(ch))
            {
                builder.Append('-');
            }
            else
            {
                builder.Append(ch);
            }
        }
    }

    internal static string? SanitizeFilePath(string? name)
    {
        if (name is null or "")
        {
            return name;
        }

        var sanitized = new char[name.Length];
        for (var index = 0; index < name.Length; index++)
        {
            var ch = name[index];
            if (IsPathInvalid(ch))
            {
                sanitized[index] = '-';
                continue;
            }

            sanitized[index] = ch;
        }

        return new(sanitized);
    }
}