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
        string? migratedExpected = null;
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
            // accepts the usual way. Only where the source was actually rewritten, so a build
            // server is left alone.
            if (InlineEngine.TryRemove(stale) &&
                stale.Expected is not null)
            {
                // The literal was the approved snapshot, so it becomes the verified file's
                // content. Otherwise the migration reads as a brand new snapshot, and the
                // approved text is lost from both the source and the failure message.
                migratedExpected = InlineEngine.NormalizeExpected(stale.Expected, stale.File);
            }

            // Prefix uniqueness is skipped while inline, since several inline verifies per
            // method are legal. Migrating puts this one back under the file naming rules, where
            // two of them in one method would resolve to the same name.
            ValidatePrefix(settings, pathPrefixReceived!);
        }

        var engine = new VerifyEngine(
            directory,
            settings,
            verifiedFiles,
            getFileNames,
            getIndexedFileNames,
            settings.TypeName ?? typeName,
            settings.MethodName ?? methodName,
            inlineEngine,
            migratedExpected);

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

        var filePairs = new List<FilePair>(engine.Equal);
        if (engine.AutoVerified.Count > 0)
        {
            filePairs.AddRange(engine.AutoVerified);
        }

        // The file pairs go with the inline result too. Only the first target is inlined, so a
        // verification with more than one still wrote files, and returning the snapshot alone left
        // a caller enumerating Files to post-process attachments seeing none of them
        if (inlineEngine is not null)
        {
            return new(inlineEngine.Rendered, filePairs, root);
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

        // Nothing was produced, so there is nothing to inline and nothing for the first target to
        // be. An explicit Snapshot call stated what the result should be and there is no result,
        // which is worth saying: the literal would otherwise be compared against nothing and the
        // verification would pass without having checked it. The global switch declines instead,
        // the way it declines every other thing it cannot do, and the verification goes on to the
        // file pipeline, which passes an empty target list the same as it always has
        if (targets.Count == 0)
        {
            if (settings.inline is null)
            {
                return null;
            }

            throw new VerifyException("Snapshot was used on a verification that produced no targets, so there is nothing to compare the snapshot against.");
        }

        // An explicit Snapshot(...) is the user's stated intent, whatever the global switch says.
        // The size limit is the one exception, and only where it was opted in to cover existing
        // calls. Not on a build server: the source cannot be rewritten there, so the literal that
        // is actually checked out is the one to compare against.
        if (settings.inline is { } explicitInline)
        {
            if (VerifierSettings.inlineApplyMaxLinesToExisting &&
                !InlineEngine.IsBuildServer() &&
                ExceedsMaxLines(targets))
            {
                return null;
            }

            return explicitInline;
        }

        if (VerifierSettings.inline is not { } globalInline)
        {
            return null;
        }

        // Hard incompatibilities. A global switch must not break unrelated tests, so these are
        // "not inline" rather than errors; the explicit path above still throws for UniqueDirectory
        // and for parameters.
        //
        // The source file is null only on the ctor that leaves typeName null too, so that check
        // reads as redundant. It is what tells the compiler the argument below is not null, so it
        // stays rather than becoming a suppression.
        if (settings.UniqueDirectory ||
            verifiedHasParameters ||
            typeName is null ||
            lineNumber == 0 ||
            inlineSourceFile is null ||
            !InlineInfo.IsSupported(inlineSourceFile))
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

        // A long snapshot swamps the test it sits in, so it stays a file
        if (ExceedsMaxLines(targets))
        {
            return null;
        }

        // No literal yet, so the patcher appends a Snapshot call to the verify invocation. The
        // line is the only thing that says which one, and it stops being true the moment another
        // accept in the same file inserts above it: a snapshot is several lines of source, call
        // sites are a handful of lines apart, and accepting a file's worth of them in one go moved
        // later hints past their own test. The member is what survives that, since the patcher
        // finds the declaration by name in the file as it is now and floors its search there.
        //
        // The declared name rather than the one naming resolved: UseMethodName renames the
        // snapshot, not the method. A name the file does not declare - a renamed test, or a
        // framework whose tests are strings rather than methods - finds no declaration and leaves
        // the search exactly as it was, so this only ever narrows
        return new(null, inlineSourceFile, lineNumber, null, methodName, InlinePatchMode.Append);
    }

    /// <summary>
    /// Whether the content that would be inlined has more lines than the configured limit.
    /// A trailing newline starts no line, so it is not counted. The content is already
    /// newline normalized.
    /// </summary>
    static bool ExceedsMaxLines(List<Target> targets)
    {
        if (VerifierSettings.inlineMaxLines is not { } maxLines ||
            targets.Count == 0 ||
            // A binary first target has no lines to count. It is not inlineable at all, and
            // InlineEngine.Compare owns that error message, so it is left to reach it.
            !targets[0].TryGetStringBuilder(out var builder))
        {
            return false;
        }

        var lines = 1;
        for (var index = 0; index < builder.Length - 1; index++)
        {
            if (builder[index] != '\n')
            {
                continue;
            }

            lines++;
            if (lines > maxLines)
            {
                return true;
            }
        }

        return false;
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