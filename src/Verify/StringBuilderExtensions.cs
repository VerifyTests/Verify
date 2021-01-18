using System.Text;

static class StringBuilderExtensions
{
    public static void FixNewlines(this StringBuilder builder)
    {
        builder.Replace("\r\n", "\n");
        builder.Replace('\r', '\n');
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