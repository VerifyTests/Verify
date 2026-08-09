// Inline counterpart of VerifyEngine. The expected content lives in the test source file, so no
// FilePair is ever created (FilePair feeds DanglingSnapshotsCheck) and accept is a source rewrite
// (via DiffEngine InlineApplier) instead of a file move.
class InlineEngine(
    string directory,
    VerifySettings settings,
    InlineInfo inline,
    IEnumerable<string> verifiedFiles,
    string? typeName,
    string? methodName)
{
    const string separator = "----------";

    bool diffEnabled = !DiffRunner.Disabled &&
                       settings.diffEnabled &&
                       !BuildServerDetector.Detected;
    string mappedSourceFile = InnerVerifier.MapSourceFile(inline.File);
    HashSet<string> delete = new(verifiedFiles, StringComparer.InvariantCultureIgnoreCase);
    Equality equality;
    string rendered = null!;
    string? normalizedExpected;

    public string RenderedText => rendered;

    public void HandleResults(List<Target> targets)
    {
        foreach (var target in targets)
        {
            if (target.IsStream)
            {
                throw new VerifyException(
                    $"""
                     Inline snapshots only support text content. A target with extension '{target.Extension}' is a binary stream.
                     Remove the Inline usage, or remove the binary target (converters, AppendFile, and stream targets all produce binary targets).
                     """);
            }
        }

        rendered = Render(targets);

        if (inline.Expected is null)
        {
            equality = Equality.New;
            return;
        }

        // The literal inherits the .cs file's line endings; mirror Comparer.Text normalization
        var expected = inline.Expected
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');

        // Mirror Comparer.CompareStrings trailing newline tolerance
        if (VerifierSettings.ignoreTrailingNewline &&
            expected.Length - 1 == rendered.Length &&
            expected[^1] == '\n')
        {
            expected = expected[..^1];
        }

        normalizedExpected = expected;
        equality = string.Equals(rendered, expected, StringComparison.Ordinal)
            ? Equality.Equal
            : Equality.NotEqual;
    }

    static string Render(List<Target> targets)
    {
        // Grouping and indexing mirror VerifyEngine.HandleResults
        var parts = new List<(string name, string content)>();
        foreach (var group in targets.GroupBy(_ => (_.Name ?? "", _.Extension)))
        {
            var list = group.ToList();
            if (list.Count == 1)
            {
                var target = list[0];
                parts.Add(($"{target.NameOrTarget}.{target.Extension}", Content(target)));
                continue;
            }

            for (var index = 0; index < list.Count; index++)
            {
                var target = list[index];
                parts.Add(($"{target.NameOrTarget}#{index:D2}.{target.Extension}", Content(target)));
            }
        }

        if (parts.Count == 1)
        {
            return parts[0].content;
        }

        return string.Join(
            "\n",
            parts.Select(_ => $"{separator} {_.name} {separator}\n{_.content}"));
    }

    static string Content(in Target target)
    {
        target.TryGetStringBuilder(out var builder);
        return builder!.ToString();
    }

    public async Task ThrowIfRequired()
    {
        if (equality == Equality.Equal)
        {
            if (diffEnabled)
            {
                // Drop anything a prior failing run left queued in the viewer
                DiffRunner.SettleInline(mappedSourceFile, inline.Line);
            }

            if (delete.Count == 0 && !Throw())
            {
                return;
            }
        }

        var allDeletesVerified = await ProcessDeletes();

        if (equality == Equality.Equal)
        {
            if (allDeletesVerified && !Throw())
            {
                return;
            }

            // Deletes-only failure: reuse the existing builder so tooling parses it unchanged
            throw new VerifyException(VerifyExceptionMessageBuilder.Build(directory, [], [], delete, []));
        }

        // New or NotEqual. The source file is passed as the delegates' verifiedFile argument
        var autoVerify = IsAutoVerify(mappedSourceFile);
        var applied = false;
        if (autoVerify && !BuildServerDetector.Detected)
        {
            // InlineApplier owns all locking (cross process mutex + in process gate)
            var result = InlineApplier.Apply(BuildPatch());
            applied = result.Status is InlineApplyStatus.Applied or InlineApplyStatus.AlreadyApplied;
        }

        if (applied)
        {
            if (allDeletesVerified && !Throw())
            {
                return;
            }
        }

        string? hint = null;
        (string Received, string Expected, string Patch)? staging = null;
        if (!applied)
        {
            // Nothing is written to disk on this path. The patch goes to DiffEngineViewer over
            // stdin, or over its socket when one is already running, and the exception message
            // below already carries the full received and expected text.
            var result = await DiffRunner.AddInlineAsync(BuildPatch());
            if (result == InlineResult.NoViewerFound)
            {
                hint = "No DiffEngineViewer was found, so the snapshot could not be opened for review. Install it: dotnet tool install -g DiffEngineViewer";
                staging = await WriteStaging();
            }
        }

        throw new VerifyException(
            VerifyExceptionMessageBuilder.BuildInline(
                directory,
                mappedSourceFile,
                inline.Line,
                isNew: equality == Equality.New,
                rendered,
                normalizedExpected,
                staging?.Received,
                staging?.Expected,
                staging?.Patch,
                delete,
                hint));
    }

    /// <summary>
    /// Fallback for when no viewer can be resolved, such as a net462 consumer or a RID with no
    /// native renderer. Writes the received and expected text so there is still something to look
    /// at, and opens whatever diff tool is configured.
    /// </summary>
    async Task<(string Received, string Expected, string Patch)?> WriteStaging()
    {
        if (BuildServerDetector.Detected ||
            VerifierSettings.IntermediateDir is null)
        {
            return null;
        }

        var stagingDirectory = Path.Combine(VerifierSettings.IntermediateDir, "VerifyInline");

        // Deterministic per call site so re-runs overwrite; runtime+version keeps
        // parallel multi-targeted runs distinct. Known accepted edge: two inline
        // verifies on the same source line share a base (staging only). The type
        // and method prefix exists only for readability in accept tooling.
        var nameBuilder = new StringBuilder();
        if (typeName is not null)
        {
            FileNameCleaner.AppendValid(nameBuilder, typeName);
            nameBuilder.Append('.');
        }

        if (methodName is not null)
        {
            FileNameCleaner.AppendValid(nameBuilder, methodName);
            nameBuilder.Append('.');
        }

        nameBuilder.Append(Fnv1a.Hash($"{mappedSourceFile}:{inline.Line}"));
        nameBuilder.Append('.');
        nameBuilder.Append(Namer.RuntimeAndVersion);
        var baseName = nameBuilder.ToString();
        var received = Path.Combine(stagingDirectory, $"{baseName}.received.txt");
        // Named .expected (not .verified) so *.verified.* tooling globs never see it
        var expected = Path.Combine(stagingDirectory, $"{baseName}.expected.txt");
        var patch = Path.Combine(stagingDirectory, $"{baseName}.inlinepatch");

        IoHelpers.WriteText(received, new(rendered));
        await VerifierSettings.RunAddTestAttachment(received);
        IoHelpers.WriteText(expected, new(normalizedExpected ?? ""));
        // Kept so the snapshot can still be reviewed by hand:
        // DiffEngineViewer --inline --source <cs> --line <n> < thisFile
        InlinePatchFile.Write(patch, BuildPatch());

        if (diffEnabled)
        {
            await DiffRunner.LaunchForTextAsync(received, expected, VerifierSettings.Encoding);
        }

        return (received, expected, patch);
    }

    InlinePatch BuildPatch() =>
        new(
            mappedSourceFile,
            inline.Line,
            inline.Expression,
            rendered);

    bool Throw() =>
        VerifierSettings.throwException ||
        settings.throwException;

    // mirrors VerifyEngine.IsAutoVerify; keep in sync
    bool IsAutoVerify(string verifiedFile)
    {
        if (typeName != null &&
            VerifierSettings.autoVerify != null)
        {
            return VerifierSettings.autoVerify(typeName, methodName!, verifiedFile);
        }

        if (settings.autoVerify != null)
        {
            return settings.autoVerify(verifiedFile);
        }

        return false;
    }

    // mirrors VerifyEngine.ProcessDeletes; keep in sync
    async Task<bool> ProcessDeletes()
    {
        var verified = true;
        foreach (var item in delete)
        {
            if (!await ProcessDelete(item))
            {
                verified = false;
            }
        }

        return verified;
    }

    async Task<bool> ProcessDelete(string file)
    {
        var autoVerify = IsAutoVerify(file);
        await settings.RunOnVerifyDelete(file, autoVerify);

        if (autoVerify)
        {
            File.Delete(file);
            return true;
        }

        await DiffRunner.AddDeleteAsync(file);

        return false;
    }
}
