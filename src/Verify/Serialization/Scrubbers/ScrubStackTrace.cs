static class ScrubStackTrace
{
    public static string Scrub(string stackTrace, bool removeParams = false)
    {
        var builder = new StringBuilder(stackTrace.Length);
        var angleBrackets = "<>".AsSpan();
        var moveNext = ".MoveNext()".AsSpan();
        var taskAwaiter = "System.Runtime.CompilerServices.TaskAwaiter".AsSpan();
        var end = "End of stack trace from previous location where exception was thrown".AsSpan();

        foreach (var line in stackTrace.AsSpan().EnumerateLines())
        {
            var span = line.TrimStart();
            if (
                span.Length == 0 ||
                (span.Contains(angleBrackets, StringComparison.Ordinal) &&
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

        if (builder.Length > 0)
        {
            builder.Length--;
        }

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