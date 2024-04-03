using System.Collections.Frozen;

namespace VerifyTests;

public static class Scrubbers
{
    static (IReadOnlyDictionary<string, string> exact, IReadOnlyDictionary<string, string> replace) machineNameReplacements;
    static (IReadOnlyDictionary<string, string> exact, IReadOnlyDictionary<string, string> replace) userNameReplacements;

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

    static (IReadOnlyDictionary<string, string> exact, IReadOnlyDictionary<string, string> replace) CreateWrappedReplacements(string toReplace, string toReplaceWith)
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

#if NET8_0_OR_GREATER
        return (exact.ToFrozenDictionary(), replace.ToFrozenDictionary());
#else
        return (exact, replace);
#endif
    }

    public static void ScrubMachineName(StringBuilder builder) =>
        PerformReplacements(builder, machineNameReplacements);

    public static void ScrubUserName(StringBuilder builder) =>
        PerformReplacements(builder, userNameReplacements);

    static void PerformReplacements(StringBuilder builder, (IReadOnlyDictionary<string, string> exact, IReadOnlyDictionary<string, string> replace) replacements)
    {
        var exactMatchingLength = replacements.exact
            .Where(_ => _.Key.Length == builder.Length)
            .ToList();
        if (exactMatchingLength.Count > 0)
        {
            var value = builder.ToString();
            foreach (var exact in exactMatchingLength)
            {
                if (value != exact.Key)
                {
                    continue;
                }

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
        var angleBrackets = "<>".AsSpan();
        var moveNext = ".MoveNext()".AsSpan();
        var taskAwaiter = "System.Runtime.CompilerServices.TaskAwaiter".AsSpan();
        var end = "End of stack trace from previous location where exception was thrown".AsSpan();

        foreach (var line in stackTrace.AsSpan().EnumerateLines())
        {
            var span = line.TrimStart();
            if ((span.Contains(angleBrackets, StringComparison.Ordinal) &&
                 span.Contains(moveNext, StringComparison.Ordinal)) ||
                span.Contains(taskAwaiter, StringComparison.Ordinal) ||
                span.Contains(end, StringComparison.Ordinal))
            {
                continue;
            }

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

            var indexOfRight = span.IndexOf(')');
            if (removeParams)
            {
                var next = indexOfLeft + 1;
                if (next == indexOfRight)
                {
                    var left = span[..(indexOfRight + 1)];
                    WriteReplacePlus(builder, left);
                }
                else
                {
                    var left = span[..next];
                    WriteReplacePlus(builder, left);
                    builder.Append("...)");
                }
            }
            else
            {
                var right = span[..(indexOfRight + 1)];
                WriteReplacePlus(builder, right);
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