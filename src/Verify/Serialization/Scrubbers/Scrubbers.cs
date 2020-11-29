using System;
using System.IO;
using System.Text;

namespace VerifyTests
{
    public static class Scrubbers
    {
        public static void ScrubMachineName(StringBuilder builder)
        {
            builder.Replace(Environment.MachineName, "TheMachineName");
        }

        public static string? ScrubStackTrace(string? stackTrace)
        {
            if (stackTrace == null)
            {
                return null;
            }

            StringBuilder builder = new();
            using StringReader reader = new(stackTrace);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (
                    line.Contains("System.Runtime.CompilerServices.TaskAwaiter") ||
                    line.Contains("End of stack trace from previous location where exception was thrown")
                )
                {
                    continue;
                }
                line = line.TrimStart();
                var indexOfRight = line.IndexOf(")");
                if (indexOfRight > -1)
                {
                    line = line.Substring(0, indexOfRight + 1);
                }

                line = line.Replace(" (", "(");
                builder.Append(line);
                builder.Append('\n');
            }
            builder.TrimEnd();
            return builder.ToString();
        }
    }
}