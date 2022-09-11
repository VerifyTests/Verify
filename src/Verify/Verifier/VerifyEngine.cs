﻿[DebuggerDisplay("new = {new.Count} | notEquals = {notEquals.Count} | equal = {equal.Count} | delete = {delete.Count}")]
class VerifyEngine
{
    string directory;
    VerifySettings settings;
    bool diffEnabled;
    List<NewResult> @new = new();
    List<NotEqualResult> notEquals = new();
    List<FilePair> equal = new();
    List<FilePair> autoVerified = new();
    HashSet<string> delete;
    GetFileNames getFileNames;
    GetIndexedFileNames getIndexedFileNames;

    public VerifyEngine(string directory, VerifySettings settings, List<string> verifiedFiles, GetFileNames getFileNames, GetIndexedFileNames getIndexedFileNames)
    {
        this.directory = directory;
        this.settings = settings;
        diffEnabled = !DiffRunner.Disabled && settings.diffEnabled;
        delete = new(verifiedFiles, StringComparer.InvariantCultureIgnoreCase);
        this.getFileNames = getFileNames;
        this.getIndexedFileNames = getIndexedFileNames;
    }

    public IReadOnlyList<FilePair> Equal => equal;
    public IReadOnlyList<FilePair> AutoVerified => autoVerified;

    static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, Target target, bool previousTextFailed)
    {
        if (target.IsStringBuilder)
        {
            var builder = target.StringBuilderData;
            return await Comparer.Text(file, builder.ToString(), settings);
        }

        if (target.IsString)
        {
            return await Comparer.Text(file, target.StringData, settings);
        }

        var stream = target.StreamData;
#if NETSTANDARD2_0 || NETFRAMEWORK || NETCOREAPP2_2 || NETCOREAPP2_1
        using (stream)
#else
        await using (stream)
#endif
        {
            stream.MoveToStart();
            return await FileComparer.DoCompare(settings, file, previousTextFailed, stream);
        }
    }

    public async Task HandleResults(List<Target> targetList)
    {
        if (targetList.Count == 1)
        {
            var target = targetList.Single();
            var file = getFileNames(target);
            var result = await GetResult(settings, file, target, false);
            HandleCompareResult(result, file);
            return;
        }

        var textHasFailed = false;

        async Task Inner(FilePair file, Target target)
        {
            var result = await GetResult(settings, file, target, textHasFailed);

            if (file.IsText &&
                result.Equality != Equality.Equal)
            {
                textHasFailed = true;
            }

            HandleCompareResult(result, file);
        }

        foreach (var group in targetList.GroupBy(_ => $"{_.Name}:{_.Extension}"))
        {
            var targets = group.ToList();
            if (targets.Count == 1)
            {
                var target = targets[0];
                var file = getFileNames(target);
                await Inner(file, target);
                continue;
            }

            for (var index = 0; index < targets.Count; index++)
            {
                var target = targets[index];
                var file = getIndexedFileNames(target, index.ToString("D2"));
                await Inner(file, target);
            }
        }
    }

    void HandleCompareResult(EqualityResult result, in FilePair file)
    {
        switch (result.Equality)
        {
            case Equality.New:
                AddMissing(new(file, result.ReceivedText));
                break;
            case Equality.NotEqual:
                AddNotEquals(new(file, result.Message, result.ReceivedText, result.VerifiedText));
                break;
            case Equality.Equal:
                AddEquals(file);
                break;
        }
    }

    void AddMissing(in NewResult item)
    {
        @new.Add(item);
        delete.Remove(item.File.VerifiedPath);
    }

    void AddNotEquals(in NotEqualResult notEqual)
    {
        notEquals.Add(notEqual);
        delete.Remove(notEqual.File.VerifiedPath);
    }

    void AddEquals(in FilePair item)
    {
        delete.Remove(item.VerifiedPath);
        equal.Add(item);
    }

    public async Task ThrowIfRequired()
    {
        ProcessEquals();
        if (@new.Count == 0 &&
            notEquals.Count == 0 &&
            delete.Count == 0)
        {
            return;
        }

        await ProcessDeletes();

        await ProcessNew();

        await ProcessNotEquals();
        if (!settings.IsAutoVerify)
        {
            var message = VerifyExceptionMessageBuilder.Build(directory, @new, notEquals, delete, equal);
            throw new VerifyException(message);
        }
    }

    Task ProcessDeletes() =>
        Task.WhenAll(delete.Select(ProcessDeletes));

    async Task ProcessDeletes(string file)
    {
        await VerifierSettings.RunOnVerifyDelete(file);

        if (settings.IsAutoVerify)
        {
            File.Delete(file);
            return;
        }

        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (DiffEngineTray.IsRunning)
        {
            await DiffEngineTray.AddDeleteAsync(file);
        }
    }

    async Task ProcessNotEquals()
    {
        foreach (var notEqual in notEquals)
        {
            await VerifierSettings.RunOnVerifyMismatch(notEqual.File, notEqual.Message);
            await RunDiffAutoCheck(notEqual.File);
        }
    }

    void ProcessEquals()
    {
        if (!diffEnabled)
        {
            return;
        }

        foreach (var item in equal)
        {
            DiffRunner.Kill(item.ReceivedPath, item.VerifiedPath);
        }
    }

    Task RunDiffAutoCheck(FilePair file)
    {
        if (settings.IsAutoVerify)
        {
            autoVerified.Add(file);
        }

        if (BuildServerDetector.Detected)
        {
            return Task.CompletedTask;
        }

        if (settings.IsAutoVerify)
        {
            AcceptChanges(file);
            return Task.CompletedTask;
        }

        if (diffEnabled)
        {
            return DiffRunner.LaunchAsync(file.ReceivedPath, file.VerifiedPath);
        }

        return Task.CompletedTask;
    }

    async Task ProcessNew()
    {
        foreach (var file in @new)
        {
            await VerifierSettings.RunOnFirstVerify(file);
            await RunDiffAutoCheck(file.File);
        }
    }

    static void AcceptChanges(in FilePair file)
    {
        File.Delete(file.VerifiedPath);
        File.Move(file.ReceivedPath, file.VerifiedPath);
    }
}