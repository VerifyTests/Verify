using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verify;

class VerifyEngine
{
    Type testType;
    string testName;
    VerifySettings settings;
    ResolvedDiffTool? diffTool;
    List<FilePair> missings = new List<FilePair>();
    List<FilePair> notEquals = new List<FilePair>();
    List<string> danglingVerified;

    public VerifyEngine(
        string extension,
        VerifySettings settings,
        Type testType,
        string directory,
        string testName)
    {
        this.settings = settings;
        this.testType = testType;
        this.testName = testName;
        diffTool = DiffTools.Find(extension);
        var verifiedPattern = FileNameBuilder.GetVerifiedPattern(extension, settings.Namer, this.testType, this.testName);
        danglingVerified = Directory.EnumerateFiles(directory, verifiedPattern).ToList();
        var receivedPattern = FileNameBuilder.GetReceivedPattern(extension, settings.Namer, this.testType, this.testName);
        foreach (var file in Directory.EnumerateFiles(directory, receivedPattern))
        {
            File.Delete(file);
        }
        diffTool = DiffTools.Find(extension);
    }

    public void HandleCompareResult(CompareResult compareResult, FilePair file)
    {
        switch (compareResult)
        {
            case CompareResult.MissingVerified:
                AddMissing(file);
                break;
            case CompareResult.NotEqual:
                AddNotEquals(file);
                break;
            case CompareResult.Equal:
                AddEquals(file);
                break;
        }
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

        if (settings.clipboardEnabled && !settings.autoVerify)
        {
            builder.AppendLine("Verify command placed in clipboard.");
        }

        await ProcessDangling(builder);

        await ProcessMissing(builder);

        await ProcessNotEquals(builder);
        if (!settings.autoVerify)
        {
            throw InnerVerifier.exceptionBuilder(builder.ToString());
        }
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
            builder.AppendLine($"  {Path.GetFileName(item)}");
            if (settings.autoVerify)
            {
                File.Delete(item);
                continue;
            }
            if (!settings.clipboardEnabled)
            {
                continue;
            }
            await ClipboardCapture.AppendDelete(item);
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
            builder.AppendLine($"{Path.GetFileName(item.Received)}");
            if (Extensions.IsTextExtension(item.Extension))
            {
                builder.AppendLine($"{File.ReadAllText(item.Received)}");
                if (File.Exists(item.Verified))
                {
                    builder.AppendLine($"{Path.GetFileName(item.Verified)}");
                    builder.AppendLine($"{File.ReadAllText(item.Verified)}");
                }
            }
            if (BuildServerDetector.Detected)
            {
                continue;
            }
            if (settings.autoVerify)
            {
                AcceptChanges(item);
                continue;
            }
            if (settings.clipboardEnabled)
            {
                await ClipboardCapture.AppendMove(item.Received, item.Verified);
            }

            if (diffTool != null &&
                settings.diffEnabled)
            {
                DiffRunner.Launch(diffTool, item.Received, item.Verified);
            }
        }
    }

    async Task ProcessMissing(StringBuilder builder)
    {
        if (!missings.Any())
        {
            return;
        }

        builder.AppendLine("Pending verification:");
        foreach (var item in missings)
        {
            builder.AppendLine($"{Path.GetFileName(item.Verified)}");
            if (Extensions.IsTextExtension(item.Extension))
            {
                builder.AppendLine($"{Path.GetFileName(item.Received)}");
                builder.AppendLine($"{File.ReadAllText(item.Received)}");
            }
            if (BuildServerDetector.Detected)
            {
                continue;
            }
            if (settings.autoVerify)
            {
                AcceptChanges(item);
                continue;
            }
            if (settings.clipboardEnabled)
            {
                await ClipboardCapture.AppendMove(item.Received, item.Verified);
            }

            if (diffTool != null &&
                settings.diffEnabled)
            {
                if (EmptyFilesWrapper.TryWriteEmptyFile(item.Extension, item.Verified))
                {
                    DiffRunner.Launch(diffTool, item.Received, item.Verified);
                }
            }
        }
    }

    static void AcceptChanges(FilePair item)
    {
        File.Delete(item.Verified);
        File.Move(item.Received, item.Verified);
    }
}