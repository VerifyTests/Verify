namespace VerifyTests;

partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets, true, true);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        var resultTargets = new List<Target>();
        if (TryGetRootTarget(root, ignoreNullRoot, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        cleanup ??= () => Task.CompletedTask;

        var (extraTargets, extraCleanup) = await GetTargets(targets, doExtensionConversion);
        cleanup = cleanup.Then(extraCleanup);
        resultTargets.AddRange(extraTargets);
        cleanup = RemoveExcludedTargets(resultTargets, cleanup, out var removedTargets);
        if (removedTargets &&
            resultTargets.Count == 0)
        {
            await cleanup();
            throw new("All targets have been excluded by ExcludeTargets. A verification requires at least one target.");
        }

        var inline = ResolveInline(resultTargets);
        InlineEngine? inlineEngine = null;
        if (inline is not null)
        {
            inlineEngine = new(
                settings,
                inline,
                settings.TypeName ?? typeName,
                settings.MethodName ?? methodName);
        }
        else if (settings.inline is { } stale)
        {
            // A literal exists but inline is off for this verification, so migrate: strip the
            // Snapshot call and let the snapshot flow through as a file, which the user then
            // accepts the usual way.
            InlineEngine.TryRemove(stale);
        }

        var engine = new VerifyEngine(
            directory,
            settings,
            verifiedFiles,
            getFileNames,
            getIndexedFileNames,
            settings.TypeName ?? typeName,
            settings.MethodName ?? methodName,
            inlineEngine);

        try
        {
            await engine.HandleResults(resultTargets);
        }
        finally
        {
            // Always run cleanup (stream/converter disposal), even if comparison throws.
            await cleanup();
        }

        await engine.ThrowIfRequired();

        if (inlineEngine is not null)
        {
            return new(inlineEngine.Rendered, root);
        }

        var filePairs = new List<FilePair>(engine.Equal);
        if (engine.AutoVerified.Count > 0)
        {
            filePairs.AddRange(engine.AutoVerified);
        }

        return new(filePairs, root);
    }

    /// <summary>
    /// Decides whether this verification uses an inline snapshot, and where the literal lives.
    /// The first target is the one inlined; the rest go through the file pipeline as usual.
    /// </summary>
    InlineInfo? ResolveInline(List<Target> targets)
    {
        if (settings.notInline)
        {
            return null;
        }

        // An explicit Snapshot(...) is the user's stated intent, whatever the global switch says
        if (settings.inline is { } explicitInline)
        {
            return explicitInline;
        }

        if (VerifierSettings.inline is not { } globalInline)
        {
            return null;
        }

        // Hard incompatibilities. A global switch must not break unrelated tests, so these are
        // "not inline" rather than errors; the explicit path above still throws for UniqueDirectory.
        if (settings.UniqueDirectory ||
            typeName is null ||
            lineNumber == 0 ||
            inlineSourceFile is null ||
            !inlineSourceFile.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var first = targets[0];
        if (first.DontInline ||
            !globalInline(
                settings.TypeName ?? typeName,
                settings.MethodName ?? methodName!,
                inlineSourceFile,
                first.Extension))
        {
            return null;
        }

        // No literal yet, so the patcher appends a Snapshot call to the verify invocation
        return new(null, inlineSourceFile, lineNumber, null, InlinePatchMode.Append);
    }

    Func<Task> RemoveExcludedTargets(List<Target> targets, Func<Task> cleanup, out bool removed)
    {
        removed = false;
        if (!VerifierSettings.AnyExcludedTargets(settings.Context))
        {
            return cleanup;
        }

        for (var index = targets.Count - 1; index >= 0; index--)
        {
            var target = targets[index];
            if (!VerifierSettings.IsExcluded(settings.Context, target.Extension))
            {
                continue;
            }

            if (target.IsStream)
            {
                // VerifyEngine disposes the streams it consumes, so an excluded stream never reaches it
                cleanup = cleanup.Then(target.StreamData.DisposeAsyncEx);
            }

            targets.RemoveAt(index);
            removed = true;
        }

        return cleanup;
    }

    async Task<(List<Target> extra, Func<Task> cleanup)> GetTargets(IEnumerable<Target> targets, bool doExtensionConversion)
    {
        List<Target> list = [..targets, ..VerifierSettings.GetFileAppenders(settings)];
        var cleanup = () => Task.CompletedTask;

        // When doExtensionConversion is false the targets have already been run through
        // conversion and scrubbing (the only caller is the post-conversion stream path),
        // so pass them through untouched to avoid double scrubbing.
        if (!doExtensionConversion)
        {
            return (list, cleanup);
        }

        var result = new List<Target>();
        foreach (var target in list)
        {
            if (!target.PerformConversion ||
                !VerifierSettings.HasStreamConverter(target.Extension))
            {
                Scrub(target);
                result.Add(target);
                continue;
            }

            var (info, converted, itemCleanup) = await DoExtensionConversion(target, null);
            cleanup = cleanup.Then(itemCleanup);
            if (info != null)
            {
                Target infoTarget = new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        info));
                Scrub(infoTarget);
                result.Add(infoTarget);
            }

            // converted targets are scrubbed within DoExtensionConversion
            result.AddRange(converted);
        }

        return (result, cleanup);
    }

    // Scrubs a text target in place. Stream (binary) targets are left untouched.
    void Scrub(in Target target)
    {
        if (target.TryGetStringBuilder(out var builder))
        {
            ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings, counter);
        }
    }

    bool TryGetRootTarget(object? root,bool ignoreNullRoot, [NotNullWhen(true)] out Target? target)
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
                target = new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        new InfoBuilder(false, stringRoot, appends)));
            }
            else
            {
                var builder = new StringBuilder(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
                target = new("txt", builder);
            }

            return true;
        }

        target = new(
            settings.TxtOrJson,
            JsonFormatter.AsJson(
                settings,
                counter,
                new InfoBuilder(ignoreNullRoot, root, appends)));
        return true;
    }
}