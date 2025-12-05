namespace VerifyTests;

partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets, true, true);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        (cleanup, var stringOrStreams) = await ProcessTargets(root, cleanup, targets, doExtensionConversion, ignoreNullRoot);
        var engine = new VerifyEngine(
            directory,
            settings,
            verifiedFiles,
            getFileNames,
            getIndexedFileNames,
            settings.TypeName ?? typeName,
            settings.MethodName ?? methodName);

        await engine.HandleResults(stringOrStreams);

        await cleanup();

        await engine.ThrowIfRequired();

        var filePairs = new List<FilePair>(engine.Equal);
        if (engine.AutoVerified.Count > 0)
        {
            filePairs.AddRange(engine.AutoVerified);
        }

        return new(filePairs, root);
    }

    async Task<(Func<Task> cleanup, List<StringOrStream> stringOrStreams)> ProcessTargets(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        var resultTargets = new List<Target>();
        if (TryGetRootTarget(root, ignoreNullRoot, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        cleanup ??= () => Task.CompletedTask;

        List<Target> list = [..targets, ..VerifierSettings.GetFileAppenders(settings)];
        if (doExtensionConversion)
        {
            var result = new List<Target>();
            foreach (var target in list)
            {
                if (!target.PerformConversion ||
                    !VerifierSettings.HasStreamConverter(target.Extension))
                {
                    result.Add(target);
                    continue;
                }

                var (info, converted, itemCleanup) = await DoExtensionConversion(target.Extension, target.StreamData, null, target.Name);
                cleanup += itemCleanup;
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
            }

            list = result;
        }

        resultTargets.AddRange(list);

        foreach (var target in resultTargets)
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings, counter);
            }
        }

        var stringOrStreams = resultTargets.Select(_ =>
            {
                if (_.IsObject)
                {
                    return new()
                    {
                        Extension = _.Extension,
                        Name = _.Name,
                        Stream = _.streamData,
                        StringBuilder = JsonFormatter.AsJson(settings, counter, _.objectData!),
                    };
                }

                return new StringOrStream
                {
                    Extension = _.Extension,
                    Name = _.Name,
                    Stream = _.streamData,
                    StringBuilder = _.stringBuilderData,
                };
            })
            .ToList();
        return (cleanup, stringOrStreams);
    }

    bool TryGetRootTarget(object? root, bool ignoreNullRoot, [NotNullWhen(true)] out Target? target)
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
                target = new(new InfoBuilder(false, stringRoot, appends), settings.TxtOrJson);
            }
            else
            {
                target = new("txt", new StringBuilder(stringRoot));
            }

            return true;
        }

        target = new(new InfoBuilder(ignoreNullRoot, root, appends), settings.TxtOrJson);
        return true;
    }
}