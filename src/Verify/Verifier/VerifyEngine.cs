using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffEngine;
using VerifyTests;

[DebuggerDisplay("missings = {missings.Count} | notEquals = {notEquals.Count} | equals = {equals.Count} | danglingVerified = {danglingVerified.Count}")]
class VerifyEngine
{
    FileNameBuilder fileNameBuilder;
    bool autoVerify;
    bool diffEnabled;
    bool clipboardEnabled;
    List<FilePair> missings = new();
    List<(FilePair filePair, string? message)> notEquals = new();
    List<FilePair> equals = new();
    List<string> danglingVerified;

    public VerifyEngine(FileNameBuilder fileNameBuilder, bool autoVerify, bool diffEnabled, bool clipboardEnabled)
    {
        this.fileNameBuilder = fileNameBuilder;
        this.autoVerify = autoVerify;
        this.diffEnabled = diffEnabled;
        this.clipboardEnabled = clipboardEnabled;
        danglingVerified = fileNameBuilder.VerifiedFiles;

        foreach (var file in fileNameBuilder.ReceivedFiles)
        {
            File.Delete(file);
        }
    }

    public async Task HandleResults(List<ResultBuilder> results)
    {
        if (results.Count == 1)
        {
            var item = results[0];
            var file = fileNameBuilder.GetFileNames(item.Extension);
            var result = await item.GetResult(file);
            HandleCompareResult(result, file);
            return;
        }

        for (var index = 0; index < results.Count; index++)
        {
            var item = results[index];
            var file = fileNameBuilder.GetFileNames(item.Extension, index);
            var result = await item.GetResult(file);
            HandleCompareResult(result, file);
        }
    }

    void HandleCompareResult(EqualityResult compareResult, in FilePair file)
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

    void AddMissing(in FilePair item)
    {
        missings.Add(item);
        danglingVerified.Remove(item.Verified);
    }

    void AddNotEquals(in FilePair item, string? message)
    {
        notEquals.Add((item, message));
        danglingVerified.Remove(item.Verified);
    }

    void AddEquals(in FilePair item)
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

        StringBuilder builder = new("Results do not match.");
        builder.AppendLine();
        if (message != null)
        {
            builder.AppendLine(message);
        }

        if (!autoVerify)
        {
            if (DiffEngineTray.IsRunning)
            {
                builder.AppendLine("Use DiffEngineTray to verify files.");
            }
            else if (ClipboardEnabled.IsEnabled())
            {
                builder.AppendLine("Verify command placed in clipboard.");
            }
        }

        await ProcessDangling(builder);

        await ProcessMissing(builder);

        await ProcessNotEquals(builder);
        if (!autoVerify)
        {
            throw new VerifyException(builder.ToString());
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
        if (autoVerify)
        {
            File.Delete(item);
            return Task.CompletedTask;
        }

        if (BuildServerDetector.Detected)
        {
            return Task.CompletedTask;
        }

        if (DiffEngineTray.IsRunning)
        {
            return DiffEngineTray.AddDeleteAsync(item);
        }

        if (ClipboardEnabled.IsEnabled())
        {
            return ClipboardCapture.AppendDelete(item);
        }

        return Task.CompletedTask;
    }

    async Task ProcessNotEquals(StringBuilder builder)
    {
        if (!notEquals.Any())
        {
            return;
        }

        builder.AppendLine("Differences:");
        foreach (var (filePair, message) in notEquals)
        {
            await ProcessNotEquals(builder, filePair, message);
        }
    }

    void ProcessEquals()
    {
        if (DiffRunner.Disabled)
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
        if (VerifierSettings.handleOnVerifyMismatch != null)
        {
            await VerifierSettings.handleOnVerifyMismatch(item, message);
        }

        builder.AppendLine($"Received: {Path.GetFileName(item.Received)}");
        builder.AppendLine($"Verified: {Path.GetFileName(item.Verified)}");
        if (message == null)
        {
            if (EmptyFiles.Extensions.IsText(item.Extension))
            {
                builder.AppendLine("Received Content:");
                builder.AppendLine($"{await FileHelpers.ReadText(item.Received)}");
                builder.AppendLine("Verified Content:");
                builder.AppendLine($"{await FileHelpers.ReadText(item.Verified)}");
            }
        }
        else
        {
            builder.AppendLine("Compare Result:");
            builder.AppendLine(message);
        }

        builder.AppendLine();

        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (autoVerify)
        {
            AcceptChanges(item);
            return;
        }

        if (clipboardEnabled)
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (diffEnabled)
        {
            await DiffRunner.LaunchAsync(item.Received, item.Verified);
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
            await ProcessMissing(builder, item);
        }
    }

    async Task ProcessMissing(StringBuilder builder, FilePair item)
    {
        if (VerifierSettings.handleOnFirstVerify != null)
        {
            await VerifierSettings.handleOnFirstVerify(item);
        }

        builder.AppendLine($"{Path.GetFileName(item.Verified)}: Empty or does not exist");
        if (EmptyFiles.Extensions.IsText(item.Extension))
        {
            builder.AppendLine($"{Path.GetFileName(item.Received)}");
            builder.AppendLine($"{await FileHelpers.ReadText(item.Received)}");
        }

        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (autoVerify)
        {
            AcceptChanges(item);
            return;
        }

        if (clipboardEnabled)
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (diffEnabled)
        {
            await DiffRunner.LaunchAsync(item.Received, item.Verified);
        }
    }

    static void AcceptChanges(in FilePair item)
    {
        File.Delete(item.Verified);
        File.Move(item.Received, item.Verified);
    }
}