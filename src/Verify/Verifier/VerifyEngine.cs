using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffEngine;
using EmptyFiles;
using Verify;

[DebuggerDisplay("missings = {missings.Count} | notEquals = {notEquals.Count} | equals = {equals.Count} | danglingVerified = {danglingVerified.Count}")]
class VerifyEngine
{
    VerifySettings settings;
    List<FilePair> missings = new List<FilePair>();
    List<(FilePair filePair, string? message)> notEquals = new List<(FilePair, string?)>();
    List<FilePair> equals = new List<FilePair>();
    List<string> danglingVerified;

    public VerifyEngine(
        string extension,
        VerifySettings settings,
        string directory,
        string testName)
    {
        this.settings = settings;
        var verifiedPattern = FileNameBuilder.GetVerifiedPattern(extension, settings.Namer, testName);
        danglingVerified = Directory.EnumerateFiles(directory, verifiedPattern).ToList();

        var receivedPattern = FileNameBuilder.GetReceivedPattern(extension, settings.Namer, testName);
        foreach (var file in Directory.EnumerateFiles(directory, receivedPattern))
        {
            File.Delete(file);
        }
    }

    public void HandleCompareResult(EqualityResult compareResult, FilePair file)
    {
        switch (compareResult.Equality)
        {
            case Equality.MissingVerified:
                AddMissing(file);
                break;
            case Equality.NotEqual:
                AddNotEquals(file, compareResult.Message);
                break;
            case Equality.Equal:
                AddEquals(file);
                break;
        }
    }

    public void AddMissing(FilePair item)
    {
        missings.Add(item);
        danglingVerified.Remove(item.Verified);
    }

    public void AddNotEquals(FilePair item,string? message)
    {
        notEquals.Add((item,message));
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

        if (ClipboardEnabled.IsEnabled(settings) && !settings.autoVerify)
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

        if (!ClipboardEnabled.IsEnabled(settings))
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
            await ProcessNotEquals(builder, item.filePair, item.message);
        }
    }

    void ProcessEquals()
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }
        foreach (var equal in equals)
        {
            DiffRunner.Kill(equal.Received, equal.Verified);
        }
    }

    async Task ProcessNotEquals(StringBuilder builder, FilePair item, string? message)
    {
        if (settings.handleOnVerifyMismatch != null)
        {
            await settings.handleOnVerifyMismatch(item.Received, item.Verified, message);
        }
        if (message != null)
        {
            builder.AppendLine($"Comparer result: {message}");
        }
        builder.AppendLine($"{Path.GetFileName(item.Received)}");
        if (Extensions.IsText(item.Extension))
        {
            builder.AppendLine($"{await FileHelpers.ReadText(item.Received)}");
            if (File.Exists(item.Verified))
            {
                builder.AppendLine($"{Path.GetFileName(item.Verified)}");
                builder.AppendLine($"{await FileHelpers.ReadText(item.Verified)}");
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

        if (ClipboardEnabled.IsEnabled(settings))
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (!settings.diffEnabled)
        {
            return;
        }

        DiffRunner.Launch(item.Received, item.Verified);
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
        if (settings.handleOnFirstVerify != null)
        {
            await settings.handleOnFirstVerify(item.Received);
        }
        builder.AppendLine($"{Path.GetFileName(item.Verified)}");
        if (Extensions.IsText(item.Extension))
        {
            builder.AppendLine($"{Path.GetFileName(item.Received)}");
            builder.AppendLine($"{await FileHelpers.ReadText(item.Received)}");
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

        if (ClipboardEnabled.IsEnabled(settings))
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (!settings.diffEnabled)
        {
            return;
        }

        DiffRunner.Launch(item.Received, item.Verified);
    }

    static void AcceptChanges(FilePair item)
    {
        File.Delete(item.Verified);
        File.Move(item.Received, item.Verified);
    }
}