using System;
using System.IO;
using System.Linq;
using System.Text;

static class LinesScrubber
{
    public static string RemoveLinesContaining(this string input, StringComparison comparison, params string[] stringToMatch)
    {
        Guard.AgainstNullOrEmpty(input, nameof(input));
        Guard.AgainstNullOrEmpty(stringToMatch, nameof(stringToMatch));
        return FilterLines(input, s => s.LineContains(stringToMatch, comparison));
    }

    public static string ReplaceLines(this string input, Func<string, string> replaceLine)
    {
        Guard.AgainstNullOrEmpty(input, nameof(input));
        Guard.AgainstNull(replaceLine, nameof(replaceLine));

        using var reader = new StringReader(input);
        var builder = new StringBuilder();

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            builder.AppendLine(replaceLine(line));
        }

        return builder.ToString();
    }
    public static string FilterLines(this string input, Func<string, bool> removeLine)
    {
        Guard.AgainstNullOrEmpty(input, nameof(input));
        Guard.AgainstNull(removeLine, nameof(removeLine));

        using var reader = new StringReader(input);
        var builder = new StringBuilder();

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (removeLine(line))
            {
                continue;
            }
            builder.AppendLine(line);
        }

        return builder.ToString();
    }

    static bool LineContains(this string line, string[] stringToMatch, StringComparison comparison)
    {
        return stringToMatch.Any(toMatch => line.IndexOf(toMatch, comparison) != -1);
    }
}