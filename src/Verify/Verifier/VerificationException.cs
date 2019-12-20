using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

partial class Verifier
{
    static Exception VerificationException(string? missing = null, string? notEqual = null, string? message = null)
    {
        var notEquals = new List<string>();
        if (notEqual != null)
        {
            notEquals.Add(notEqual);
        }

        var missings = new List<string>();
        if (missing != null)
        {
            missings.Add(missing);
        }

        return VerificationException(missings, notEquals, message);
    }

    static Exception VerificationException(List<string> missings, List<string> notEquals, string? message = null)
    {
        var builder = new StringBuilder("Results do not match.");
        builder.AppendLine();
        if (message != null)
        {
            builder.AppendLine(message);
        }

        if (!BuildServerDetector.Detected)
        {
            builder.AppendLine("Verification command copied to the clipboard.");
        }

        if (missings.Any())
        {
            builder.AppendLine("Not verified:");
            foreach (var item in missings)
            {
                builder.AppendLine($"  {item}");
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLine("Differences:");
            foreach (var item in notEquals)
            {
                builder.AppendLine($"  {item}");
            }
        }

        return exceptionBuilder(builder.ToString());
    }
}