using DiffEngine;

[DebuggerDisplay("new = {new.Count} | notEqual = {notEqual.Count} | equal = {equal.Count} | delete = {delete.Count}")]
class VerifyEngine
{
    string directory;
    VerifySettings settings;
    bool diffEnabled;
    List<FilePair> @new = new();
    List<(FilePair filePair, string? message)> notEqual = new();
    List<FilePair> equal = new();
    List<string> delete;
    GetFileNames getFileNames;
    GetIndexedFileNames getIndexedFileNames;

    public VerifyEngine(string directory, VerifySettings settings, List<string> verifiedFiles, GetFileNames getFileNames, GetIndexedFileNames getIndexedFileNames)
    {
        this.directory = directory;
        this.settings = settings;
        diffEnabled = !DiffRunner.Disabled && settings.diffEnabled;
        delete = verifiedFiles;
        this.getFileNames = getFileNames;
        this.getIndexedFileNames = getIndexedFileNames;
    }

    static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, Target target, bool previousTextFailed)
    {
        if (target.IsStringBuilder)
        {
            var builder = target.StringBuilderData;
            ApplyScrubbers.Apply(target.Extension, builder, settings);
            return await Comparer.Text(file, builder.ToString(), settings);
        }

        if (target.IsString)
        {
            var builder = new StringBuilder(target.StringData);
            ApplyScrubbers.Apply(target.Extension, builder, settings);
            return await Comparer.Text(file, builder.ToString(), settings);
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
            if (file.IsText &&
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
            case Equality.New:
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
        @new.Add(item);
        delete.Remove(item.VerifiedPath);
    }

    void AddNotEquals(in FilePair item, string? message)
    {
        notEqual.Add((item, message));
        delete.Remove(item.VerifiedPath);
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
            notEqual.Count == 0 &&
            delete.Count == 0)
        {
            return;
        }

        await ProcessDeletes();

        await ProcessNew();

        await ProcessNotEquals();
        if (!settings.autoVerify)
        {
            var message = await VerifyExceptionMessageBuilder.Build(directory, @new, notEqual, delete, equal);
            throw new VerifyException(message);
        }
    }

    async Task ProcessDeletes()
    {
        foreach (var item in delete)
        {
            await ProcessDeletes(item);
        }
    }

    async Task ProcessDeletes(string file)
    {
        await VerifierSettings.RunOnVerifyDelete(file);

        if (settings.autoVerify)
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
        foreach (var (file, message) in notEqual)
        {
            await VerifierSettings.RunOnVerifyMismatch(file, message);
            await RunDiffAutoCheck(file);
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

    async Task RunDiffAutoCheck(FilePair file)
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        if (settings.autoVerify)
        {
            AcceptChanges(file);
            return;
        }

        if (diffEnabled)
        {
            await DiffRunner.LaunchAsync(file.ReceivedPath, file.VerifiedPath);
        }
    }

    async Task ProcessNew()
    {
        foreach (var file in @new)
        {
            await VerifierSettings.RunOnFirstVerify(file);
            await RunDiffAutoCheck(file);
        }
    }

    static void AcceptChanges(in FilePair file)
    {
        File.Delete(file.VerifiedPath);
        File.Move(file.ReceivedPath, file.VerifiedPath);
    }
}