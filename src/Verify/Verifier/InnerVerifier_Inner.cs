partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> fileTargets)
    {
        var targetList = GetTargetList(fileTargets).ToList();

        if (TryGetTargetBuilder(root, out var builder, out var extension))
        {
            if (extension is null)
            {
                extension = VerifierSettings.TxtOrJson;
            }

            var received = builder.ToString();
            var stream = new Target(extension, received);
            targetList.Insert(0, stream);
        }

        targetList.AddRange(VerifierSettings.GetFileAppenders(settings));

        return RunEngine(root, cleanup, targetList);
    }

    Task<VerifyResult> VerifyInner(string? root, Func<Task>? cleanup, IEnumerable<Target> fileTargets)
    {
        var targetList = GetTargetList(fileTargets).ToList();

        if (TryGetTargetBuilderString(root, out var builder, out var extension))
        {
            if (targetList.Any(_ => _.IsStream))
            {
                // if there are stream targets, extension applies to stream, and "target" is just text metadata.
                extension = "txt";
            }
            else if (extension is null)
            {
                extension = VerifierSettings.TxtOrJson;
            }

            var received = builder.ToString();
            var stream = new Target(extension, received);
            targetList.Insert(0, stream);
        }

        targetList.AddRange(VerifierSettings.GetFileAppenders(settings));

        return RunEngine(root, cleanup, targetList);
    }

    bool TryGetTargetBuilder(object? target, [NotNullWhen(true)] out StringBuilder? builder, out string? extension)
    {
        extension = null;
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (target is null)
        {
            if (!hasAppends)
            {
                builder = null;
                return false;
            }

            builder = JsonFormatter.AsJson(null, appends, settings, counter);
            return true;
        }

        builder = JsonFormatter.AsJson(target, appends, settings, counter);

        return true;
    }

    bool TryGetTargetBuilderString(string? target, [NotNullWhen(true)] out StringBuilder? builder, out string? extension)
    {
        extension = null;
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (target is null)
        {
            if (!hasAppends)
            {
                builder = null;
                return false;
            }

            builder = JsonFormatter.AsJson(null, appends, settings, counter);
            return true;
        }

        target = target.TrimPreamble();

        if (!hasAppends)
        {
            builder = new(target);
            extension = settings.ExtensionOrTxt();
            ApplyScrubbers.ApplyForExtension(extension, builder, settings);
            return true;
        }

        builder = JsonFormatter.AsJson(target, appends, settings, counter);

        return true;
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