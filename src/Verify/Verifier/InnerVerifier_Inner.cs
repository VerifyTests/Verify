namespace VerifyTests;

partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets, true, true);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        cleanup ??= () => Task.CompletedTask;
        var (resultTargets, extraCleanup) = await GetTargets(root, targets, doExtensionConversion, ignoreNullRoot);
        cleanup += extraCleanup;
        var engine = new VerifyEngine(
            directory,
            settings,
            verifiedFiles,
            getFileNames,
            getIndexedFileNames,
            settings.TypeName ?? typeName,
            settings.MethodName ?? methodName);

        await engine.HandleResults(resultTargets);

        await cleanup();

        await engine.ThrowIfRequired();

        var filePairs = new List<FilePair>(engine.Equal);
        if (engine.AutoVerified.Count > 0)
        {
            filePairs.AddRange(engine.AutoVerified);
        }

        return new(filePairs, root);
    }

    async Task<(List<Target> resultTargets, Func<Task> extraCleanup)> GetTargets(object? root, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        var resultTargets = new List<Target>();
        if (TryGetRootTarget(root, ignoreNullRoot, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        var (extraTargets, extraCleanup) = await GetTargets(targets, doExtensionConversion);
        resultTargets.AddRange(extraTargets);
        return (resultTargets, extraCleanup);
    }

    async Task<(List<Target> extra, Func<Task> cleanup)> GetTargets(IEnumerable<Target> targets, bool doExtensionConversion)
    {
        List<Target> list = [..targets, ..VerifierSettings.GetFileAppenders(settings)];
        var cleanup = () => Task.CompletedTask;
        if (doExtensionConversion)
        {
            var result = new List<Target>();
            foreach (var target in list)
            {
                var itemCleanup = await ProcessTarget(target, result);
                cleanup += itemCleanup;
            }

            list = result;
        }

        foreach (var target in list)
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings, counter);
            }
        }

        return (list, cleanup);
    }

    private async Task<Func<Task>?> ProcessTarget(Target target, List<Target> result)
    {
        if (!target.PerformConversion ||
            !VerifierSettings.HasStreamConverter(target.Extension))
        {
            result.Add(target);
            return null;
        }

        var (info, converted, cleanup) = await DoExtensionConversion(target.Extension, target.StreamData, null, target.Name);
        if (info != null)
        {
            result.Add(
                new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        info)));
        }

        result.AddRange(converted);
        return cleanup;
    }

    bool TryGetRootTarget(object? root,bool ignoreNullRoot, [NotNullWhen(true)] out Target? target)
    {
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Count > 0;

        if (ignoreNullRoot && root == null && !hasAppends)
        {
            target = null;
            return false;
        }

        if (root is string stringRoot)
        {
            stringRoot = stringRoot.TrimPreamble();
            if (stringRoot.Length == 0)
            {
                stringRoot = "emptyString";
            }

            if (hasAppends)
            {
                target = new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        new InfoBuilder(false, stringRoot, appends)));
            }
            else
            {
                var builder = new StringBuilder(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
                target = new("txt", builder);
            }

            return true;
        }

        target = new(
            settings.TxtOrJson,
            JsonFormatter.AsJson(
                settings,
                counter,
                new InfoBuilder(ignoreNullRoot, root, appends)));
        return true;
    }
}