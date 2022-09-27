﻿partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets, true);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExpressionConversion)
    {
        var resultTargets = new List<Target>();
        if (TryGetRootTarget(root, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        resultTargets.AddRange(await GetTargets(targets, doExpressionConversion));
        var engine = new VerifyEngine(directory, settings, verifiedFiles, getFileNames, getIndexedFileNames);

        await engine.HandleResults(resultTargets);

        if (cleanup is not null)
        {
            await cleanup();
        }

        await engine.ThrowIfRequired();
        return new(engine.Equal.Concat(engine.AutoVerified).ToList(), root);
    }

    async Task<List<Target>> GetTargets(IEnumerable<Target> targets, bool doExpressionConversion)
    {
        var list = targets.Concat(VerifierSettings.GetFileAppenders(settings))
            .ToList();
        if (doExpressionConversion)
        {
            var result = new List<Target>();
            foreach (var target in list)
            {
                if (VerifierSettings.HasExtensionConverter(target.Extension))
                {
                    var (info, converted, cleanup) = await DoExtensionConversion(target.Extension, target.StreamData);

                    result.AddRange(converted);
                }
                else
                {
                    result.Add(target);
                }
            }

            list = result;
        }

        foreach (var target in list)
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings);
            }
        }

        return list;
    }

    bool TryGetRootTarget(object? root, [NotNullWhen(true)] out Target? target)
    {
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (root is null)
        {
            if (hasAppends)
            {
                target = new(
                    VerifierSettings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        new InfoBuilder(null, appends)));
                return true;
            }

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
                    VerifierSettings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        new InfoBuilder(stringRoot, appends)));
            }
            else
            {
                var builder = new StringBuilder(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings);
                target = new("txt", builder);
            }

            return true;
        }

        target = new(
            VerifierSettings.TxtOrJson,
            JsonFormatter.AsJson(
                settings,
                counter,
                new InfoBuilder(root, appends)));
        return true;
    }
}