﻿partial class InnerVerifier
{
    async Task<VerifyResult> VerifyInner(object? target, Func<Task>? cleanup, IEnumerable<Target> targets)
    {
        var targetList = GetTargetList(targets).ToList();

        if (TryGetTargetBuilder(target, out var builder, out var extension))
        {
            if (target is string &&
                targetList.Any(item => item.IsStream))
            {
                // if we have stream targets, extension applies to stream, and "target" is just text metadata.
                extension = "txt";
            }

            var received = builder.ToString();
            var stream = new Target(extension, received);
            targetList.Insert(0, stream);
        }

        targetList.AddRange(VerifierSettings.GetFileAppenders(settings));

        var engine = new VerifyEngine(directory, settings, verifiedFiles, getFileNames, getIndexedFileNames);

        await engine.HandleResults(targetList);

        if (cleanup is not null)
        {
            await cleanup();
        }

        await engine.ThrowIfRequired();
        return new(engine.Equal, target);
    }

    IEnumerable<Target> GetTargetList(IEnumerable<Target> targets)
    {
        foreach (var target in targets)
        {
            if (target.IsStringBuilder)
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, target.StringBuilderData, settings);
                yield return new(target.Extension, target.StringBuilderData, target.Name);
            }
            else if (target.IsString)
            {
                var builder = new StringBuilder(target.StringData);
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings);
                yield return new(target.Extension, builder, target.Name);
            }
            else
            {
                yield return target;
            }
        }
    }

    bool TryGetTargetBuilder(object? target, [NotNullWhen(true)] out StringBuilder? builder, [NotNullWhen(true)] out string? extension)
    {
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Any();

        if (target is null)
        {
            if (!hasAppends)
            {
                builder = null;
                extension = null;
                return false;
            }

            extension = "txt";
            if (VerifierSettings.StrictJson)
            {
                extension = "json";
            }

            builder = JsonFormatter.AsJson(null, appends, settings, counter);

            return true;
        }

        if (target is string stringTarget)
        {
            target = stringTarget = TrimPreamble(stringTarget);

            if (!hasAppends)
            {
                builder = new(stringTarget);
                extension = settings.ExtensionOrTxt();
                ApplyScrubbers.ApplyForExtension(extension, builder, settings);
                return true;
            }
        }

        extension = "txt";

        if (VerifierSettings.StrictJson)
        {
            extension = "json";
        }

        builder = JsonFormatter.AsJson(target, appends, settings, counter);

        return true;
    }

    static string TrimPreamble(string text) =>
        text.TrimStart('\uFEFF');
}