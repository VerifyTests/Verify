namespace VerifyTests;

public static class Scrubbers
{
    static (Dictionary<string, string> exact, Dictionary<string, string> replace) machineNameReplacements;
    static (Dictionary<string, string> exact, Dictionary<string, string> replace) userNameReplacements;

    static Scrubbers() =>
        ResetReplacements(Environment.MachineName, Environment.UserName);

    internal static void ResetReplacements(string machineName, string userName)
    {
        machineNameReplacements = CreateWrappedReplacements(machineName, "TheMachineName");
        userNameReplacements = CreateWrappedReplacements(userName, "TheUserName");
    }

    static char[] validWrappingChars =
    [
        ' ',
        '\t',
        '\n',
        '\r'
    ];

    static (Dictionary<string, string> exact, Dictionary<string, string> replace) CreateWrappedReplacements(string toReplace, string toReplaceWith)
    {
        var replace = new Dictionary<string, string>(validWrappingChars.Length * 2);
        foreach (var wrappingChar in validWrappingChars)
        {
            replace[wrappingChar + toReplace] = wrappingChar + toReplaceWith;
            replace[toReplace + wrappingChar] = toReplaceWith + wrappingChar;
        }

        var exact = new Dictionary<string, string>(2 + validWrappingChars.Length * validWrappingChars.Length)
        {
            {
                toReplace, toReplaceWith
            }
        };
        foreach (var beforeChar in validWrappingChars)
        foreach (var afterChar in validWrappingChars)
        {
            exact[beforeChar + toReplace + afterChar] = beforeChar + toReplaceWith + afterChar;
        }

        return (exact, replace);
    }

    public static void ScrubMachineName(StringBuilder builder) =>
        PerformReplacements(builder, machineNameReplacements);

    public static void ScrubUserName(StringBuilder builder) =>
        PerformReplacements(builder, userNameReplacements);

    static void PerformReplacements(StringBuilder builder, (Dictionary<string, string> exact, Dictionary<string, string> replace) replacements)
    {
        var value = builder.ToString();
        foreach (var exact in replacements.exact)
        {
            if (value == exact.Key)
            {
                builder.Clear();
                builder.Append(exact.Value);
                return;
            }
        }

        foreach (var replace in replacements.replace)
        {
            builder.ReplaceIfLonger(replace.Key, replace.Value);
        }
    }

    public static string ScrubStackTrace(string stackTrace, bool removeParams = false)
    {
        var builder = new StringBuilder();
        using var reader = new StringReader(stackTrace);
        while (reader.ReadLine() is { } line)
        {
            if (
                (line.Contains("<>") && line.Contains(".MoveNext()")) ||
                line.Contains("System.Runtime.CompilerServices.TaskAwaiter") ||
                line.Contains("End of stack trace from previous location where exception was thrown")
            )
            {
                continue;
            }

            line = line.TrimStart();
            if (!line.StartsWith("at "))
            {
                builder.AppendLineN(line);
                continue;
            }

            if (line.StartsWith("at InnerVerifier.Throws") ||
                line.StartsWith("at InnerVerifier.<Throws"))
            {
                continue;
            }

            if (removeParams)
            {
                var indexOfLeft = line.IndexOf('(');
                if (indexOfLeft > -1)
                {
                    var c = line[indexOfLeft + 1];
                    if (c == ')')
                    {
                        line = line[..(indexOfLeft + 2)];
                    }
                    else
                    {
                        line = line[..(indexOfLeft + 1)] + "...)";
                    }
                }
            }
            else
            {
                var indexOfRight = line.IndexOf(')');
                if (indexOfRight > -1)
                {
                    line = line[..(indexOfRight + 1)];
                }
            }

            line = line.Replace(" (", "(");
            line = line.Replace('+', '.');
            builder.AppendLineN(line);
        }

        builder.TrimEnd();
        return builder.ToString();
    }
}