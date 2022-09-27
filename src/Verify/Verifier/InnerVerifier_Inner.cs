partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets)
    {
        var resultTargets = new List<Target>();
        if (TryGetRootTarget(root, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        resultTargets.AddRange(GetTargets(targets));
        var engine = new VerifyEngine(directory, settings, verifiedFiles, getFileNames, getIndexedFileNames);

        await engine.HandleResults(resultTargets);

        if (cleanup is not null)
        {
            await cleanup();
        }

        await engine.ThrowIfRequired();
        return new(engine.Equal.Concat(engine.AutoVerified).ToList(), root);
    }

    IEnumerable<Target> GetTargets(IEnumerable<Target> targets)
    {
        foreach (var target in targets.Concat(VerifierSettings.GetFileAppenders(settings)))
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings);
            }

            yield return target;
        }
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