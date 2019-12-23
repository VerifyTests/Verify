using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

partial class Verifier
{
    static Exception VerificationException(FilePair? missing = null, string? notEqual = null, string? message = null)
    {
        var notEquals = new List<string>();
        if (notEqual != null)
        {
            notEquals.Add(notEqual);
        }

        var missings = new List<FilePair>();
        if (missing != null)
        {
            missings.Add(missing);
        }

        return VerificationException(missings, notEquals, new List<string>(), message);
    }

    static Exception VerificationException(List<FilePair> missings, List<string> notEquals, List<string> danglingVerified, string? message = null)
    {
        var builder = new StringBuilder("Results do not match.");
        builder.AppendLine();
        if (message != null)
        {
            builder.AppendLine(message);
        }

        if (!BuildServerDetector.Detected)
        {
            builder.AppendLine("Verify command placed in clipboard.");
        }

        if (missings.Any())
        {
            builder.AppendLine("Pending:");
            foreach (var item in missings)
            {
                builder.AppendLine($"  {Path.GetFileName(item.Verified)}");
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLine("Differences:");
            foreach (var item in notEquals)
            {
                builder.AppendLine($"  {Path.GetFileName(item)}");
            }
        }

        if (danglingVerified.Any())
        {
            builder.AppendLine("Deletions:");
            foreach (var item in danglingVerified)
            {
                builder.AppendLine($"  {Path.GetFileName(item)}");
            }
        }

        return exceptionBuilder(builder.ToString());
    }
}