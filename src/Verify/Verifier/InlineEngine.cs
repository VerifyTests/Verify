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
        var stagingDirectory = VerifierSettings.IntermediateDir is null
            ? null
            : Path.Combine(VerifierSettings.IntermediateDir, "VerifyInline");
        var stagingAvailable = !BuildServerDetector.Detected && stagingDirectory is not null;
        string? receivedStaged = null;
        string? expectedStaged = null;
        string? patchStaged = null;
        if (stagingAvailable)
        {
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
            receivedStaged = Path.Combine(stagingDirectory!, $"{baseName}.received.txt");
            // Named .expected (not .verified) so *.verified.* tooling globs never see it
            expectedStaged = Path.Combine(stagingDirectory!, $"{baseName}.expected.txt");
            patchStaged = Path.Combine(stagingDirectory!, $"{baseName}.inlinepatch");

            // Fresh state per run. On a passing run this also settles any pending
            // tray item: the tray drops inline moves whose staging files vanished
            if (Directory.Exists(stagingDirectory))
            {
                File.Delete(receivedStaged);
                File.Delete(expectedStaged);
                File.Delete(patchStaged);
            }
        }

        if (equality == Equality.Equal)
        {
            if (diffEnabled && stagingAvailable)
            {
                // Close any diff tool left from a prior failing run
                DiffRunner.Kill(receivedStaged!, expectedStaged!);
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
        if (!applied && stagingAvailable)
        {
            IoHelpers.WriteText(receivedStaged!, new StringBuilder(rendered));
            await VerifierSettings.RunAddTestAttachment(receivedStaged!);
            IoHelpers.WriteText(expectedStaged!, new StringBuilder(normalizedExpected ?? ""));
            InlinePatchFile.Write(patchStaged!, BuildPatch());
            var moveResult = await DiffRunner.AddInlineMoveAsync(receivedStaged!, mappedSourceFile, patchStaged!, expectedStaged);
            if (moveResult == InlineMoveResult.TrayTooOld)
            {
                hint = "The running DiffEngineTray predates inline snapshot support. Update it: dotnet tool update -g DiffEngineTray";
            }

            if (diffEnabled)
            {
                await DiffRunner.LaunchForTextAsync(receivedStaged!, expectedStaged!, VerifierSettings.Encoding);
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
                applied ? null : receivedStaged,
                applied ? null : expectedStaged,
                applied ? null : patchStaged,
                delete,
                hint));
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
