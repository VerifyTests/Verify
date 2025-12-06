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
        if (root == null)
        {
            if (!ignoreNullRoot)
            {
                targets = [new((object?) null, settings.TxtOrJson, null), ..targets];
            }
        }
        else
        {
            targets = [new(root, settings.TxtOrJson, null), ..targets];
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

        var appends = VerifierSettings.GetJsonAppenders(settings);
        var hasAppends = appends.Count > 0;
        if (hasAppends)
        {
            var resultTarget = resultTargets[0];
            if (resultTarget.IsStream)
            {
                var target = new Target(new InfoBuilder(ignoreNullRoot, null, appends), settings.TxtOrJson);
                resultTargets.Insert(0, target);
            }
            else if (resultTarget.IsString)
            {
                var stringData = resultTarget.stringBuilderData!;
                string stringRoot;
                if (stringData.Length == 0)
                {
                    stringRoot = "emptyString";
                }
                else
                {
                    stringRoot = stringData.ToString();
                }

                var target = new Target(new InfoBuilder(false, stringRoot, appends), settings.TxtOrJson);
                resultTargets.Insert(0, target);
            }
            else
            {
                var target = new Target(new InfoBuilder(ignoreNullRoot, resultTarget.objectData!, appends), settings.TxtOrJson);
                resultTargets.Insert(0, target);
            }
        }

        foreach (var target in resultTargets)
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings, counter);
            }
        }

        var stringOrStreams = resultTargets
            .Select(_ =>
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
}