using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class Verifier
{
    static Task<Exception> VerificationException(FilePair? missing = null, FilePair? notEqual = null, string? message = null)
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

        return VerificationException(missings, notEquals, new List<string>(), message);
    }

    static async Task<Exception> VerificationException(List<FilePair> missings, List<FilePair> notEquals, List<string> danglingVerified, string? message = null)
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

        await ProcessMissing(missings, builder);

        await ProcessNotEquals(notEquals, builder);

        ProcessDangling(danglingVerified, builder);

        return exceptionBuilder(builder.ToString());
    }

    static void ProcessDangling(List<string> danglingVerified, StringBuilder builder)
    {
        if (!danglingVerified.Any())
        {
            return;
        }
        builder.AppendLine("Deletions:");
        foreach (var item in danglingVerified)
        {
            builder.AppendLine($"  {Path.GetFileName(item)}");
        }
    }

    static async Task ProcessNotEquals(List<FilePair> notEquals, StringBuilder builder)
    {
        if (!notEquals.Any())
        {
            return;
        }

        builder.AppendLine("Differences:");
        foreach (var item in notEquals)
        {
            if (!BuildServerDetector.Detected)
            {
                await ClipboardCapture.AppendMove(item.Received, item.Verified);

                if (DiffTools.TryFindForExtension(item.Extension, out var diffTool))
                {
                    DiffRunner.Launch(diffTool, item.Received, item.Verified);
                }
            }

            builder.AppendLine($"  {Path.GetFileName(item.Received)}");
        }
    }

    static async Task ProcessMissing(List<FilePair> missings, StringBuilder builder)
    {
        if (!missings.Any())
        {
            return;
        }

        builder.AppendLine("Pending:");
        foreach (var item in missings)
        {
            if (!BuildServerDetector.Detected)
            {
                await ClipboardCapture.AppendMove(item.Received, item.Verified);
                
                if (DiffTools.TryFindForExtension(item.Extension, out var diffTool))
                {
                    if (EmptyFiles.TryWriteEmptyFile(item.Extension, item.Verified))
                    {
                        DiffRunner.Launch(diffTool, item.Received, item.Verified);
                    }
                }
            }

            builder.AppendLine($"  {Path.GetFileName(item.Verified)}");
        }
    }
}