partial class InnerVerifier
{
    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> fileTargets)
    {
        var targetList = GetTargets(root, fileTargets)
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
                var builder = JsonFormatter.AsJson(null, appends, settings, counter);
                var received = builder.ToString();
                yield return new(VerifierSettings.TxtOrJson, received);
            }
        }
        else if (root is string stringRoot)
        {
            stringRoot = stringRoot.TrimPreamble();

            if (hasAppends)
            {
                var builder = JsonFormatter.AsJson(stringRoot, appends, settings, counter);
                yield return new(VerifierSettings.TxtOrJson, builder.ToString());
            }
            else
            {
                var builder = new StringBuilder(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings);
                yield return new("txt", builder.ToString());
            }
        }
        else
        {
            yield return new(VerifierSettings.TxtOrJson, JsonFormatter.AsJson(root, appends, settings, counter));
        }

        foreach (var target in targets)
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

        foreach (var target in VerifierSettings.GetFileAppenders(settings))
        {
            yield return target;
        }
    }
}