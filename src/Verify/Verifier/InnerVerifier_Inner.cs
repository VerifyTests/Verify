namespace VerifyTests;

partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets, true, true);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        var resultTargets = new List<Target>();
        if (TryGetRootTarget(root, ignoreNullRoot, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        cleanup ??= () => Task.CompletedTask;

        var (extraTargets, extraCleanup) = await GetTargets(targets, doExtensionConversion);
        cleanup = cleanup.Then(extraCleanup);
        resultTargets.AddRange(extraTargets);
        var engine = new VerifyEngine(
            directory,
            settings,
            verifiedFiles,
            getFileNames,
            getIndexedFileNames,
            settings.TypeName ?? typeName,
            settings.MethodName ?? methodName);

        try
        {
            await engine.HandleResults(resultTargets);
        }
        finally
        {
            // Always run cleanup (stream/converter disposal), even if comparison throws.
            await cleanup();
        }

        await engine.ThrowIfRequired();

        var filePairs = new List<FilePair>(engine.Equal);
        if (engine.AutoVerified.Count > 0)
        {
            filePairs.AddRange(engine.AutoVerified);
        }

        return new(filePairs, root);
    }

    async Task<(List<Target> extra, Func<Task> cleanup)> GetTargets(IEnumerable<Target> targets, bool doExtensionConversion)
    {
        List<Target> list = [..targets, ..VerifierSettings.GetFileAppenders(settings)];
        var cleanup = () => Task.CompletedTask;

        // When doExtensionConversion is false the targets have already been run through
        // conversion and scrubbing (the only caller is the post-conversion stream path),
        // so pass them through untouched to avoid double scrubbing.
        if (!doExtensionConversion)
        {
            return (list, cleanup);
        }

        var result = new List<Target>();
        foreach (var target in list)
        {
            if (!target.PerformConversion ||
                !VerifierSettings.HasStreamConverter(target.Extension))
            {
                Scrub(target);
                result.Add(target);
                continue;
            }

            var (info, converted, itemCleanup) = await DoExtensionConversion(target, null);
            cleanup = cleanup.Then(itemCleanup);
            if (info != null)
            {
                Target infoTarget = new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        info));
                Scrub(infoTarget);
                result.Add(infoTarget);
            }

            // converted targets are scrubbed within DoExtensionConversion
            result.AddRange(converted);
        }

        return (result, cleanup);
    }

    // Scrubs a text target in place. Stream (binary) targets are left untouched.
    void Scrub(in Target target)
    {
        if (target.TryGetStringBuilder(out var builder))
        {
            ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings, counter);
        }
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