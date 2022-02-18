namespace VerifyTests;

public static class Scrubbers
{
    public static void ScrubMachineName(StringBuilder builder)
    {
        builder.Replace(Environment.MachineName, "TheMachineName");
    }

    public static string? ScrubStackTrace(string? stackTrace, bool removeParams = false)
    {
        if (stackTrace is null)
        {
            return null;
        }

        var builder = new StringBuilder();
        using var reader = new StringReader(stackTrace);
        while (reader.ReadLine() is { } line)
        {
            if (
                line.Contains("<>") && line.Contains(".MoveNext()") ||
                line.Contains("System.Runtime.CompilerServices.TaskAwaiter") ||
                line.Contains("End of stack trace from previous location where exception was thrown")
            )
            {
                continue;
            }

            line = line.TrimStart();
            if (!line.StartsWith("at "))
            {
                builder.Append(line);
                builder.Append('\n');
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
            builder.Append(line);
            builder.Append('\n');
        }

        builder.TrimEnd();
        return builder.ToString();
    }
}