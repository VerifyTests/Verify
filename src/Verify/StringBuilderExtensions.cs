namespace VerifyTests;

public static class StringBuilderExtensions
{
    public static void FixNewlines(this StringBuilder builder)
    {
        builder.Replace("\r\n", "\n");
        builder.Replace('\r', '\n');
    }

    /// <summary>
    /// Appends a line with a `\n` as the newline character.
    /// </summary>
    public static StringBuilder AppendLineN(this StringBuilder builder)
    {
        builder.Append('\n');
        return builder;
    }

    /// <summary>
    /// Appends a line with a `\n` as the newline character.
    /// </summary>
    public static StringBuilder AppendLineN(this StringBuilder builder, string? value)
    {
        builder.Append(value);
        builder.Append('\n');
        return builder;
    }

    /// <summary>
    /// Appends a line with a `\n` as the newline character.
    /// </summary>
    public static StringBuilder AppendLineN(this StringBuilder builder, StringBuilder? value)
    {
        if (value != null)
        {
            builder.Append(value);
        }

        builder.Append('\n');
        return builder;
    }

    public static void TrimEnd(this StringBuilder builder)
    {
        if (builder.Length == 0)
        {
            return;
        }

        var i = builder.Length - 1;
        for (; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(builder[i]))
            {
                break;
            }
        }

        if (i < builder.Length - 1)
        {
            builder.Length = i + 1;
        }
    }
}