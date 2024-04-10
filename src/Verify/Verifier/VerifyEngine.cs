// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable UseAwaitUsing

[DebuggerDisplay("new = {new.Count} | notEquals = {notEquals.Count} | equal = {equal.Count} | delete = {delete.Count}")]
class VerifyEngine
{
    string directory;
    VerifySettings settings;
    bool diffEnabled;
    List<NewResult> @new = [];
    List<NotEqualResult> notEquals = [];
    List<FilePair> equal = [];
    List<FilePair> autoVerified = [];
    HashSet<string> delete;
    GetFileNames getFileNames;
    GetIndexedFileNames getIndexedFileNames;
    string? typeName;
    string? methodName;

    public VerifyEngine(
        string directory,
        VerifySettings settings,
        IEnumerable<string> verifiedFiles,
        GetFileNames getFileNames,
        GetIndexedFileNames getIndexedFileNames,
        string? typeName,
        string? methodName)
    {
        this.directory = directory;
        this.settings = settings;
#if DiffEngine
        diffEnabled = !DiffRunner.Disabled && settings.diffEnabled;
#endif
        delete = new(verifiedFiles, StringComparer.InvariantCultureIgnoreCase);
        this.getFileNames = getFileNames;
        this.getIndexedFileNames = getIndexedFileNames;
        this.typeName = typeName;
        this.methodName = methodName;
    }

    public IReadOnlyList<FilePair> Equal => equal;
    public IReadOnlyList<FilePair> AutoVerified => autoVerified;

    static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, Target target, bool previousTextFailed)
    {
        if (target.TryGetStringBuilder(out var value))
        {
            return await Comparer.Text(file, value, settings);
        }

        using (var stream = target.StreamData)
        {
            stream.MoveToStart();
            return await FileComparer.DoCompare(settings, file, previousTextFailed, stream);
        }
    }

    public async Task HandleResults(List<Target> targetList)
    {
        if (targetList.Count == 1)
        {
            var target = targetList[0];
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

    void HandleCompareResult(EqualityResult result, FilePair file)
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

        var allDeletesDerified = await ProcessDeletes();

        var allNewVerified = await ProcessNew();

        var allNotEqualsVerified = await ProcessNotEquals();

        if (allDeletesDerified && allNewVerified && allNotEqualsVerified)
        {
            return;
        }

        var message = VerifyExceptionMessageBuilder.Build(directory, @new, notEquals, delete, equal);
        throw new VerifyException(message, notEquals.Select(ne => ne.File).ToList());
    }

    internal bool IsAutoVerify(string verifiedFile)
    {
        if (typeName == null)
        {
            return false;
        }

        if (VerifierSettings.autoVerify != null)
        {
            return VerifierSettings.autoVerify(typeName, methodName!, verifiedFile);
        }

        if (settings.autoVerify != null)
        {
            return settings.autoVerify(verifiedFile);
        }

        return false;
    }

    async Task<bool> ProcessDeletes()
    {
        var verified = true;
        foreach (var item in delete)
        {
           if(!await ProcessDeletes(item))
           {
               verified = false;
           }
        }

        return verified;
    }

    async Task<bool> ProcessDeletes(string file)
    {
        await VerifierSettings.RunOnVerifyDelete(file);

        if (IsAutoVerify(file))
        {
            File.Delete(file);
            return true;
        }

#if DiffEngine
        if (BuildServerDetector.Detected)
        {
            return false;
        }

        if (DiffEngineTray.IsRunning)
        {
            await DiffEngineTray.AddDeleteAsync(file);
        }
#endif
        return false;
    }

    async Task<bool> ProcessNotEquals()
    {

        var verified = true;
        foreach (var notEqual in notEquals)
        {
            await VerifierSettings.RunOnVerifyMismatch(notEqual.File, notEqual.Message);
            if (!await RunDiffAutoCheck(notEqual.File))
            {
                verified = false;
            }
        }

        return verified;
    }

    void ProcessEquals()
    {
        if (!diffEnabled)
        {
            return;
        }

#if DiffEngine
        foreach (var item in equal)
        {
            DiffRunner.Kill(item.ReceivedPath, item.VerifiedPath);
        }
#endif
    }

    // ReSharper disable once UnusedParameter.Local
    // ReSharper disable once MemberCanBeMadeStatic.Local
    async Task<bool> RunDiffAutoCheck(FilePair file)
    {
#if DiffEngine
        var autoVerify = IsAutoVerify(file.VerifiedPath);
        if (autoVerify)
        {
            autoVerified.Add(file);
        }

        if (BuildServerDetector.Detected)
        {
            return autoVerify;
        }

        if (autoVerify)
        {
            AcceptChanges(file);
            return autoVerify;
        }

        if (diffEnabled)
        {
            await DiffRunner.LaunchAsync(file.ReceivedPath, file.VerifiedPath, VerifierSettings.Encoding);
        }
#endif

        return autoVerify;
    }

    async Task<bool> ProcessNew()
    {
        var verified = true;
        foreach (var file in @new)
        {
            await VerifierSettings.RunOnFirstVerify(file);
            if (!await RunDiffAutoCheck(file.File))
            {
                verified = false;
            }
        }

        return verified;
    }

    static void AcceptChanges(in FilePair file)
    {
        File.Delete(file.VerifiedPath);
        File.Move(file.ReceivedPath, file.VerifiedPath);
    }
}