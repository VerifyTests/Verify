using System.Text;

static class StringBuilderExtensions
{
    public static bool Compare(this StringBuilder builder1, StringBuilder builder2)
    {
        if (builder1.Length != builder2.Length)
        {
            return false;
        }

        for (var index = 0; index < builder1.Length; index++)
        {
            var char1 = builder1[index];
            var char2 = builder2[index];
            if (char1 != char2)
            {
                return false;
            }
        }

        return true;
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