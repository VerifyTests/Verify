static class LinesScrubber
{
    public static void RemoveLinesContaining(this StringBuilder input, StringComparison comparison, params string[] stringToMatch)
    {
        Guard.NotNullOrEmpty(stringToMatch);
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
    }

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

        if (theString.Length > 0 &&
            !theString.EndsWith('\n'))
        {
            input.Length -= 1;
        }
    }
}