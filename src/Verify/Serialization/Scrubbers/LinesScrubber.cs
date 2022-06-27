﻿static class LinesScrubber
{
    public static void RemoveLinesContaining(this StringBuilder input, StringComparison comparison, params string[] stringToMatch)
    {
        Guard.AgainstNullOrEmpty(stringToMatch, nameof(stringToMatch));
        FilterLines(input, s => s.LineContains(stringToMatch, comparison));
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
                input.Append(value);
                input.Append('\n');
            }
        }

        if (theString.Length > 0 &&
            !theString.EndsWith("\n"))
        {
            input.Length -= 1;
        }
    }

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

            input.Append(line);
            input.Append('\n');
        }

        if (input.Length > 0 &&
            !theString.EndsWith("\n"))
        {
            input.Length -= 1;
        }
    }

    static bool LineContains(this string line, string[] stringToMatch, StringComparison comparison) =>
        stringToMatch.Any(toMatch => line.IndexOf(toMatch, comparison) != -1);
}