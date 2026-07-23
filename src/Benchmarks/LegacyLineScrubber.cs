static class LegacyLineScrubber
{
    public static void FilterLines(this StringBuilder input, Func<string, bool> removeLine)
    {
        var theString = input.ToString();
        using var reader = new StringReader(theString);
        input.Clear();

        while (reader.ReadLine() is { } line)
        {
            if (removeLine(line))
            {
                continue;
            }

            input.AppendLineN(line);
        }

        var endsWithNewLine = theString.EndsWith('\n');
        if (input.Length > 0 && !endsWithNewLine)
        {
            input.Length -= 1;
        }
    }

    public static void RemoveEmptyLines(this StringBuilder builder)
    {
        builder.FilterLines(string.IsNullOrWhiteSpace);
        if (builder.Length > 0 &&
            builder[0] is '\n')
        {
            builder.Remove(0, 1);
        }

        if (builder.Length > 0 &&
            builder[^1] is '\n')
        {
            builder.Length--;
        }
    }

    public static void RemoveLinesContaining(this StringBuilder input, StringComparison comparison, params string[] stringToMatch) =>
        input.FilterLines(_ =>
        {
            foreach (var toMatch in stringToMatch)
            {
                if (_.Contains(toMatch, comparison))
                {
                    return true;
                }
            }

            return false;
        });

    public static void ReplaceLines(this StringBuilder input, Func<string, string?> replaceLine)
    {
        var theString = input.ToString();
        using var reader = new StringReader(theString);
        input.Clear();
        while (reader.ReadLine() is { } line)
        {
            var value = replaceLine(line);
            if (value is not null)
            {
                input.AppendLineN(value);
            }
        }

        if (input.Length > 0 &&
            !theString.EndsWith('\n'))
        {
            input.Length -= 1;
        }
    }
}