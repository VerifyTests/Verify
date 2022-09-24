﻿partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> fileTargets)
    {
        var targetList = new List<Target>();

        if (TryGetTargetBuilder(root, out var builder))
        {
            var received = builder.ToString();
            targetList.Add(new(VerifierSettings.TxtOrJson, received));
        }

        targetList.AddRange(GetTargetList(fileTargets));
        return RunEngine(root, cleanup, targetList);
    }

    bool TryGetTargetBuilder(object? target, [NotNullWhen(true)] out StringBuilder? builder)
    {
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

    Task<VerifyResult> VerifyInnerString(string root)
    {
        var target = root;
        StringBuilder builder;
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        target = target.TrimPreamble();

        var extension = VerifierSettings.TxtOrJson;
        if (hasAppends)
        {
            builder = JsonFormatter.AsJson(target, appends, settings, counter);
        }
        else
        {
            builder = new(target);
            extension = "txt";
            ApplyScrubbers.ApplyForExtension("txt", builder, settings);
        }

        var targetList = new List<Target>
        {
            new(extension, builder.ToString())
        };

        return RunEngine(root, null, targetList);
    }

    bool TryGetTargetWithAppends(object? target, [NotNullWhen(true)] out object? result)
    {
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (target is null)
        {
            if (!hasAppends)
            {
                result = null;
                return false;
            }

            var infoBuilder = new InfoBuilder();

            foreach (var append in appends)
            {
                infoBuilder.Add(append.Name, append.Data);
            }

            result = infoBuilder;
            return true;
        }
        else
        {
            if (!hasAppends)
            {
                result = target;
                return true;
            }

            var infoBuilder = new InfoBuilder();
            infoBuilder.Add("target", target);

            foreach (var append in appends)
            {
                infoBuilder.Add(append.Name, append.Data);
            }

            result = infoBuilder;
            return true;
        }
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