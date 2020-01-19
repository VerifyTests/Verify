using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verify;

class VerifyEngine
{
    string extension;
    VerifySettings settings;
    List<FilePair> missings = new List<FilePair>();
    List<FilePair> notEquals = new List<FilePair>();
    List<FilePair> equals = new List<FilePair>();
    List<string> danglingVerified;

    public VerifyEngine(
        string extension,
        VerifySettings settings,
        Type testType,
        string directory,
        string testName)
    {
        this.extension = extension;
        this.settings = settings;
        var verifiedPattern = FileNameBuilder.GetVerifiedPattern(extension, settings.Namer, testType, testName);
        danglingVerified = Directory.EnumerateFiles(directory, verifiedPattern).ToList();
        var receivedPattern = FileNameBuilder.GetReceivedPattern(extension, settings.Namer, testType, testName);
        foreach (var file in Directory.EnumerateFiles(directory, receivedPattern))
        {
            File.Delete(file);
        }
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
        equals.Add(item);
    }

    public async Task ThrowIfRequired(string? message = null)
    {
        ProcessEquals();
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
            await ProcessDangling(builder, item);
        }
    }

    Task ProcessDangling(StringBuilder builder, string item)
    {
        builder.AppendLine($"  {Path.GetFileName(item)}");
        if (settings.autoVerify)
        {
            File.Delete(item);
            return Task.CompletedTask;
        }

        if (!settings.clipboardEnabled)
        {
            return Task.CompletedTask;
        }

        return ClipboardCapture.AppendDelete(item);
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
            await ProcessNotEquals(builder, item);
        }
    }

    void ProcessEquals()
    {
        foreach (var equal in equals)
        {
            if (DiffTools.TryFind(extension, out var diffTool))
            {
                DiffRunner.KillProcessIfSupported(diffTool, equal);
            }
        }
    }

    async Task ProcessNotEquals(StringBuilder builder, FilePair item)
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
            return;
        }

        if (settings.autoVerify)
        {
            AcceptChanges(item);
            return;
        }

        if (settings.clipboardEnabled)
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (!settings.diffEnabled)
        {
            return;
        }

        if (!DiffTools.TryFind(extension, out var diffTool))
        {
            return;
        }

        DiffRunner.Launch(diffTool, item);
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
            await ProcessMissing(builder, item);
        }
    }

    async Task ProcessMissing(StringBuilder builder, FilePair item)
    {
        builder.AppendLine($"{Path.GetFileName(item.Verified)}");
        if (Extensions.IsTextExtension(item.Extension))
        {
            builder.AppendLine($"{Path.GetFileName(item.Received)}");
            builder.AppendLine($"{File.ReadAllText(item.Received)}");
        }

        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (settings.autoVerify)
        {
            AcceptChanges(item);
            return;
        }

        if (settings.clipboardEnabled)
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (!settings.diffEnabled)
        {
            return;
        }

        if (!DiffTools.TryFind(extension, out var diffTool))
        {
            return;
        }

        if (!EmptyFilesWrapper.TryWriteEmptyFile(item.Extension, item.Verified))
        {
            return;
        }

        DiffRunner.Launch(diffTool, item);
    }

    static void AcceptChanges(FilePair item)
    {
        File.Delete(item.Verified);
        File.Move(item.Received, item.Verified);
    }
}