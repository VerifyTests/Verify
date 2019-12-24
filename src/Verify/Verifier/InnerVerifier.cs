using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class InnerVerifier
{
    ResolvedDiffTool? diffTool;
    List<FilePair> missings = new List<FilePair>();
    List<FilePair> notEquals = new List<FilePair>();
    List<string> danglingVerified;

    public InnerVerifier(string extension, IEnumerable<string> existingVerified)
    {
        danglingVerified = existingVerified.ToList();
        DiffTools.TryFindForExtension(extension, out diffTool);
    }

    public InnerVerifier(string extension)
    {
        danglingVerified = new List<string>();
        DiffTools.TryFindForExtension(extension, out diffTool);
    }

    public void AddMissing(FilePair item)
    {
        missings.Add(item);
        danglingVerified.Remove(item.Verified);
    }

    public void AddNotEquals(FilePair item)
    {
        notEquals.Add(item);
        danglingVerified.Remove(item.Verified);
    }

    public void AddEquals(FilePair item)
    {
        danglingVerified.Remove(item.Verified);
    }

    public async Task ThrowIfRequired(string? message = null)
    {
        if (missings.Count == 0 &&
            notEquals.Count == 0 &&
            danglingVerified.Count == 0)
        {
            return;
        }

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

        await ProcessDangling(builder);

        await ProcessMissing(builder);

        await ProcessNotEquals(builder);

        throw Verifier.exceptionBuilder(builder.ToString());
    }

    async Task ProcessDangling(StringBuilder builder)
    {
        if (!danglingVerified.Any())
        {
            return;
        }

        builder.AppendLine("Deletions:");
        foreach (var item in danglingVerified)
        {
            await ClipboardCapture.AppendDelete(item);
            builder.AppendLine($"  {Path.GetFileName(item)}");
        }
    }

    async Task ProcessNotEquals(StringBuilder builder)
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

                if (diffTool != null)
                {
                    DiffRunner.Launch(diffTool, item.Received, item.Verified);
                }
            }

            builder.AppendLine($"  {Path.GetFileName(item.Received)}");
        }
    }

    async Task ProcessMissing(StringBuilder builder)
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

                if (diffTool != null)
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