partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> fileTargets)
    {
        var targetList = new List<Target>();

        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (root is null)
        {
            if (hasAppends)
            {
                var builder = JsonFormatter.AsJson(null, appends, settings, counter);
                var received = builder.ToString();
                targetList.Add(new(VerifierSettings.TxtOrJson, received));
            }
        }
        else if (root is string stringRoot)
        {
            stringRoot = stringRoot.TrimPreamble();

            if (hasAppends)
            {
                var builder = JsonFormatter.AsJson(stringRoot, appends, settings, counter);
                targetList.Add(new(VerifierSettings.TxtOrJson, builder.ToString()));
            }
            else
            {
                StringBuilder builder = new(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings);
                targetList.Add(new("txt", builder.ToString()));
            }
        }
        else
        {
            var builder = JsonFormatter.AsJson(root, appends, settings, counter);

            var received = builder.ToString();
            targetList.Add(new(VerifierSettings.TxtOrJson, received));
        }

        targetList.AddRange(GetTargetList(fileTargets));
        return RunEngine(root, cleanup, targetList);
    }

    IEnumerable<Target> GetTargetList(IEnumerable<Target> targets)
    {
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
    }

    async Task<VerifyResult> RunEngine(object? root, Func<Task>? cleanup, List<Target> targetList)
    {
        targetList.AddRange(VerifierSettings.GetFileAppenders(settings));
        var engine = new VerifyEngine(directory, settings, verifiedFiles, getFileNames, getIndexedFileNames);

        await engine.HandleResults(targetList);

        if (cleanup is not null)
        {
            await cleanup();
        }

        await engine.ThrowIfRequired();
        return new(engine.Equal.Concat(engine.AutoVerified).ToList(), root);
    }
}