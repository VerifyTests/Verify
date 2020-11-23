using System;
using System.IO;
using System.Linq;
using System.Text;

static class LinesScrubber
{
    public static void RemoveLinesContaining(this StringBuilder input, StringComparison comparison, params string[] stringToMatch)
    {
        Guard.AgainstNull(input, nameof(input));
        Guard.AgainstNullOrEmpty(stringToMatch, nameof(stringToMatch));
        FilterLines(input, s => s.LineContains(stringToMatch, comparison));
    }

    public static void ReplaceLines(this StringBuilder input, Func<string, string> replaceLine)
    {
        Guard.AgainstNull(input, nameof(input));
        Guard.AgainstNull(replaceLine, nameof(replaceLine));

        var theString = input.ToString();
        using StringReader reader = new(theString);
        input.Clear();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            input.Append(replaceLine(line));
            input.Append('\n');
        }

        if (!theString.EndsWith("\n"))
        {
            input.Length -= 1;
        }
    }

    public static void FilterLines(this StringBuilder input, Func<string, bool> removeLine)
    {
        Guard.AgainstNull(input, nameof(input));
        Guard.AgainstNull(removeLine, nameof(removeLine));

        var theString = input.ToString();
        using StringReader reader = new(theString);
        input.Clear();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (removeLine(line))
            {
                continue;
            }

            input.Append(line);
            input.Append('\n');
        }

        if (!theString.EndsWith("\n"))
        {
            input.Length -= 1;
        }
    }

    static bool LineContains(this string line, string[] stringToMatch, StringComparison comparison)
    {
        return stringToMatch.Any(toMatch => line.IndexOf(toMatch, comparison) != -1);
    }
}