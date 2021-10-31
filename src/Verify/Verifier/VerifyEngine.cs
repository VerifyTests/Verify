using DiffEngine;
using VerifyTests;

[DebuggerDisplay("missings = {missings.Count} | notEquals = {notEquals.Count} | equals = {equals.Count} | danglingVerified = {danglingVerified.Count}")]
class VerifyEngine
{
    VerifySettings settings;
    bool diffEnabled;
    static bool clipboardEnabled = !DiffEngineTray.IsRunning && ClipboardEnabled.IsEnabled();
    List<FilePair> missings = new();
    List<(FilePair filePair, string? message)> notEquals = new();
    List<FilePair> equals = new();
    List<string> danglingVerified;
    GetFileNames getFileNames;
    GetIndexedFileNames getIndexedFileNames;

    public VerifyEngine(VerifySettings settings, List<string> verifiedFiles, GetFileNames getFileNames, GetIndexedFileNames getIndexedFileNames)
    {
        this.settings = settings;
        diffEnabled = !DiffRunner.Disabled && settings.diffEnabled;
        danglingVerified = verifiedFiles;
        this.getFileNames = getFileNames;
        this.getIndexedFileNames = getIndexedFileNames;
    }

    static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair filePair, Target target, bool previousTextHasFailed)
    {
        if (target.IsStringBuilder)
        {
            var builder = target.StringBuilderData;
            ApplyScrubbers.Apply(target.Extension, builder, settings);
            return await Comparer.Text(filePair, builder.ToString(), settings);
        }

        if (target.IsString)
        {
            StringBuilder builder = new(target.StringData);
            ApplyScrubbers.Apply(target.Extension, builder, settings);
            return await Comparer.Text(filePair, builder.ToString(), settings);
        }

        var stream = target.StreamData;
#if NETSTANDARD2_0 || NETFRAMEWORK
        using (stream)
#else
        await using (stream)
#endif
        {
            stream.MoveToStart();
            return await Comparer.Streams(settings, stream, filePair, previousTextHasFailed);
        }
    }

    public async Task HandleResults(List<Target> targetList)
    {
        if (targetList.Count == 1)
        {
            var target = targetList.Single();
            var file = getFileNames(target.Extension);
            var result = await GetResult(settings, file, target, false);
            HandleCompareResult(result, file);
            return;
        }

        var textHasFailed = false;
        for (var index = 0; index < targetList.Count; index++)
        {
            var target = targetList[index];
            var file = getIndexedFileNames(target.Extension, index);
            var result = await GetResult(settings, file, target, textHasFailed);
            if (EmptyFiles.Extensions.IsText(target.Extension) &&
                result.Equality != Equality.Equal)
            {
                textHasFailed = true;
            }

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

        var builder = new StringBuilder("Results do not match.");
        builder.AppendLine();
        if (message is not null)
        {
            builder.AppendLine(message);
        }

        if (!settings.autoVerify)
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
        if (!settings.autoVerify)
        {
            throw new VerifyException(builder.ToString());
        }
    }

    async Task ProcessDangling(StringBuilder builder)
    {
        if (danglingVerified.IsEmpty())
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
        if (notEquals.IsEmpty())
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
        await VerifierSettings.RunOnVerifyMismatch(item, message);

        builder.AppendLine($"Received: {Path.GetFileName(item.Received)}");
        builder.AppendLine($"Verified: {Path.GetFileName(item.Verified)}");
        if (message is null)
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

        await RunClipboardDiffAutoCheck(item);
    }

    async Task ProcessMissing(StringBuilder builder, FilePair item)
    {
        await VerifierSettings.RunOnFirstVerify(item);

        builder.AppendLine($"{Path.GetFileName(item.Verified)}: Empty or does not exist");
        if (EmptyFiles.Extensions.IsText(item.Extension))
        {
            builder.AppendLine($"{Path.GetFileName(item.Received)}");
            builder.AppendLine($"{await FileHelpers.ReadText(item.Received)}");
        }

        await RunClipboardDiffAutoCheck(item);
    }

    async Task RunClipboardDiffAutoCheck(FilePair item)
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (settings.autoVerify)
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
        if (missings.IsEmpty())
        {
            return;
        }

        builder.AppendLine("Pending verification:");
        foreach (var item in missings)
        {
            await ProcessMissing(builder, item);
        }
    }
    
    static void AcceptChanges(in FilePair item)
    {
        File.Delete(item.Verified);
        File.Move(item.Received, item.Verified);
    }
}