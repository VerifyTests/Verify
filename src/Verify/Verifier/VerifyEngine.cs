﻿using System.Collections.Generic;
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
    VerifySettings settings;
    FileNameBuilder fileNameBuilder;
    List<FilePair> missings = new();
    List<(FilePair filePair, string? message)> notEquals = new();
    List<FilePair> equals = new();
    List<string> danglingVerified;

    public VerifyEngine(
        VerifySettings settings,
        FileNameBuilder fileNameBuilder)
    {
        this.settings = settings;
        this.fileNameBuilder = fileNameBuilder;
        danglingVerified = fileNameBuilder.GetVerifiedFiles().ToList();

        foreach (var file in fileNameBuilder.GetReceivedFiles())
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

    public void HandleCompareResult(EqualityResult compareResult, in FilePair file)
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

        if (!settings.autoVerify)
        {
            if (DiffEngineTray.IsRunning)
            {
                builder.AppendLine("Use DiffEngineTray to verify files.");
            }
            else if (ClipboardEnabled.IsEnabled(settings))
            {
                builder.AppendLine("Verify command placed in clipboard.");
            }
        }

        await ProcessDangling(builder);

        await ProcessMissing(builder);

        await ProcessNotEquals(builder);
        if (!settings.autoVerify)
        {
            throw new(builder.ToString());
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

    async Task ProcessDangling(StringBuilder builder, string item)
    {
        builder.AppendLine($"  {Path.GetFileName(item)}");
        if (settings.autoVerify)
        {
            File.Delete(item);
            return;
        }

        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (DiffEngineTray.IsRunning)
        {
            await DiffEngineTray.AddDeleteAsync(item);
            return;
        }

        if (!ClipboardEnabled.IsEnabled(settings))
        {
            return;
        }

        await ClipboardCapture.AppendDelete(item);
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
        if (settings.handleOnVerifyMismatch != null)
        {
            await settings.handleOnVerifyMismatch(item, message);
        }

        if (message != null)
        {
            builder.AppendLine($"Comparer result: {message}");
        }

        builder.AppendLine($"{Path.GetFileName(item.Received)}");
        if (EmptyFiles.Extensions.IsText(item.Extension))
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

        if (!DiffEngineTray.IsRunning &&
            ClipboardEnabled.IsEnabled(settings))
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (!settings.diffEnabled)
        {
            return;
        }

        await DiffRunner.LaunchAsync(item.Received, item.Verified);
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
            await settings.handleOnFirstVerify(item);
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

        if (settings.autoVerify)
        {
            AcceptChanges(item);
            return;
        }

        if (!DiffEngineTray.IsRunning &&
            ClipboardEnabled.IsEnabled(settings))
        {
            await ClipboardCapture.AppendMove(item.Received, item.Verified);
        }

        if (!settings.diffEnabled)
        {
            return;
        }

        await DiffRunner.LaunchAsync(item.Received, item.Verified);
    }

    static void AcceptChanges(in FilePair item)
    {
        File.Delete(item.Verified);
        File.Move(item.Received, item.Verified);
    }
}