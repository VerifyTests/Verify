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
        var builder = new StringBuilder(stackTrace.Length);
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

            var span = line.AsSpan();
            span = span.TrimStart();
            if (!span.StartsWith("at "))
            {
                builder.AppendLineN(span);
                continue;
            }

            if (span.StartsWith("at InnerVerifier.Throws") ||
                span.StartsWith("at InnerVerifier.<Throws"))
            {
                continue;
            }

            var indexOfLeft = span.IndexOf('(');

            if (indexOfLeft == -1)
            {
                WriteReplacePlus(builder, span);
            }
            else
            {
                var indexOfRight = span.IndexOf(')');
                if (removeParams)
                {
                    if (indexOfLeft + 1 == indexOfRight)
                    {
                        var left = span[..(indexOfRight + 1)];
                        WriteReplacePlus(builder, left);
                    }
                    else
                    {
                        var left = span[..(indexOfLeft + 1)];
                        WriteReplacePlus(builder, left);
                        builder.Append("...)");
                    }
                }
                else
                {
                    var right = span[..(indexOfRight + 1)];
                    WriteReplacePlus(builder, right);
                }
            }

            builder.AppendLineN();
        }

        builder.Length--;
        return builder.ToString();
    }

    static void WriteReplacePlus(StringBuilder builder, CharSpan span)
    {
        while (true)
        {
            var indexOf = span.IndexOf('+');
            if (indexOf == -1)
            {
                builder.Append(span);
                return;
            }

            builder.Append(span[..indexOf]);
            builder.Append('.');
            span = span[(indexOf + 1)..];
        }
    }
}