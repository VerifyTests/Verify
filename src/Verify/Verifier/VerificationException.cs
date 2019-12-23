using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class Verifier
{
    static Task<Exception> VerificationException(Func<FilePair, Task> diff, FilePair? missing = null, FilePair? notEqual = null, string? message = null)
    {
        var notEquals = new List<FilePair>();
        if (notEqual != null)
        {
            notEquals.Add(notEqual);
        }

        var missings = new List<FilePair>();
        if (missing != null)
        {
            missings.Add(missing);
        }

        return VerificationException(diff, missings, notEquals, new List<string>(), message);
    }

    static async Task<Exception> VerificationException(Func<FilePair, Task> diff, List<FilePair> missings, List<FilePair> notEquals, List<string> danglingVerified, string? message = null)
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

            foreach (var item in danglingVerified)
            {
                await ClipboardCapture.AppendDelete(item);
            }
        }

        if (missings.Any())
        {
            builder.AppendLine("Pending:");
            foreach (var item in missings)
            {
                if (!BuildServerDetector.Detected)
                {
                    await ClipboardCapture.AppendMove(item.Received, item.Verified);
                    await diff(item);
                }

                builder.AppendLine($"  {Path.GetFileName(item.Verified)}");
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLine("Differences:");
            foreach (var item in notEquals)
            {
                if (!BuildServerDetector.Detected)
                {
                    await ClipboardCapture.AppendMove(item.Received, item.Verified);
                    await diff(item);
                }

                builder.AppendLine($"  {Path.GetFileName(item.Received)}");
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