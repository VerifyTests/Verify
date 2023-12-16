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

    public static string ReplaceInvalidFileNameChars(this string value)
    {
        var chars = value.ToCharArray();

        var found = false;
        for (var index = 0; index < chars.Length; index++)
        {
            var ch = chars[index];
            if (invalidFileNameChars.Contains(ch))
            {
                found = true;
                chars[index] = '-';
            }
        }

        if (found)
        {
            return new(chars);
        }

        return value;
    }

    public static void AppendValid(StringBuilder builder, string value)
    {
        foreach (var ch in value)
        {
            if (invalidFileNameChars.Contains(ch))
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