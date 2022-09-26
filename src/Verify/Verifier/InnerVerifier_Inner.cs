partial class InnerVerifier
{
    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets)
    {
        var targetList = GetTargets(root, targets)
            .ToList();
        var engine = new VerifyEngine(directory, settings, verifiedFiles, getFileNames, getIndexedFileNames);

        await engine.HandleResults(targetList);

        if (cleanup is not null)
        {
            await cleanup();
        }

        await engine.ThrowIfRequired();
        return new(engine.Equal.Concat(engine.AutoVerified).ToList(), root);
    }

    IEnumerable<Target> GetTargets(object? root, IEnumerable<Target> targets)
    {
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (root is null)
        {
            if (hasAppends)
            {
                var infoBuilder = new InfoBuilder(null, appends);
                yield return new(
                    VerifierSettings.TxtOrJson,
                    JsonFormatter.AsJson(settings, counter, infoBuilder));
            }
        }
        else if (root is string stringRoot)
        {
            stringRoot = stringRoot.TrimPreamble();
            if (stringRoot.Length == 0)
            {
                stringRoot = "emptyString";
            }

            if (hasAppends)
            {
                var infoBuilder = new InfoBuilder(stringRoot, appends);
                yield return new(
                    VerifierSettings.TxtOrJson,
                    JsonFormatter.AsJson(settings, counter, infoBuilder));
            }
            else
            {
                var builder = new StringBuilder(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings);
                yield return new("txt", builder);
            }
        }
        else
        {
            var infoBuilder = new InfoBuilder(root, appends);
            yield return new(
                VerifierSettings.TxtOrJson,
                JsonFormatter.AsJson(settings, counter, infoBuilder));
        }

        foreach (var target in targets.Concat(VerifierSettings.GetFileAppenders(settings)))
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings);
                yield return new(target.Extension, builder, target.Name);
            }
            else
            {
                yield return target;
            }
        }
    }
}