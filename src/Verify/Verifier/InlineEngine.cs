// The inline half of VerifyEngine. One target's expected content lives in the test source file
// instead of a .verified file, so no FilePair is created (FilePair feeds DanglingSnapshotsCheck)
// and accept is a source rewrite (via DiffEngine InlineApplier) instead of a file move.
// Owned and driven by VerifyEngine, which keeps deletes, auto verify and the exception message.
class InlineEngine(
    VerifySettings settings,
    InlineInfo inline,
    string? typeName,
    string? methodName)
{
    bool diffEnabled = !DiffRunner.Disabled &&
                       settings.diffEnabled &&
                       !IsBuildServer();

    /// <summary>
    /// Swapped in tests. Whether a patch reached the viewer is otherwise unobservable: the result
    /// is Queued either way, and there is no way to ask the viewer what it was sent.
    /// </summary>
    internal static Func<InlinePatch, Task<InlineResult>> AddInline = _ => DiffRunner.AddInlineAsync(_);

    /// <summary>
    /// Swapped in tests. Every source rewrite below is a no-op on a build server, so the tests that
    /// cover those rewrites need the check off. Scoped to inline rather than moving
    /// BuildServerDetector.Detected, which is global and would reach tests running in parallel.
    /// </summary>
    internal static Func<bool> IsBuildServer = () => BuildServerDetector.Detected;

    public string MappedSourceFile { get; } = InnerVerifier.MapSourceFile(inline.File);
    public int Line => inline.Line;
    public Equality Equality { get; private set; }
    public string Rendered { get; private set; } = null!;
    public string? NormalizedExpected { get; private set; }

    public void Compare(in Target target)
    {
        if (target.IsStream)
        {
            throw new VerifyException(
                $"""
                 Inline snapshots only support text content. The first target, with extension '{target.Extension}', is a binary stream.
                 Use `.NotInline()` for this test, or `Target.DontInline` for this extension.
                 """);
        }

        target.TryGetStringBuilder(out var builder);
        Rendered = builder!.ToString();

        if (inline.Expected is null)
        {
            Equality = Equality.New;
            return;
        }

        var expected = NormalizeExpected(inline.Expected);

        // Mirror Comparer.CompareStrings trailing newline tolerance
        if (VerifierSettings.ignoreTrailingNewline &&
            expected.Length - 1 == Rendered.Length &&
            expected[^1] == '\n')
        {
            expected = expected[..^1];
        }

        NormalizedExpected = expected;
        Equality = string.Equals(Rendered, expected, StringComparison.Ordinal)
            ? Equality.Equal
            : Equality.NotEqual;
    }

    /// <summary>
    /// The literal inherits the .cs file's line endings; mirror Comparer.Text normalization.
    /// Also used when a literal is migrated into a verified file, so the two can never drift.
    /// </summary>
    internal static string NormalizeExpected(string expected) =>
        expected
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');

    /// <summary>
    /// Drops anything a prior failing run left queued in the viewer.
    /// </summary>
    public void Settle()
    {
        if (diffEnabled)
        {
            DiffRunner.SettleInline(MappedSourceFile, inline.Line);
        }
    }

    /// <summary>
    /// Rewrites the source in place. Only used by auto verify, and by the migration away from
    /// inline, both of which are decided rather than reviewed.
    /// </summary>
    public bool TryApply()
    {
        if (IsBuildServer())
        {
            return false;
        }

        // InlineApplier owns all locking (cross process mutex + in process gate)
        var result = InlineApplier.Apply(BuildPatch());
        return result.Status is InlineApplyStatus.Applied or InlineApplyStatus.AlreadyApplied;
    }

    /// <summary>
    /// Hands the snapshot to DiffEngineViewer for review. Nothing is written to disk: an already
    /// running viewer receives the patch over its socket, and a newly launched one over stdin.
    /// Returns the staging paths only when no viewer could be found.
    /// </summary>
    public async Task<(string? Hint, StagedInline? Staged)> Queue()
    {
        // Queueing is the inline equivalent of launching a diff tool, so it answers to the same
        // switches. Without this a test that disables diff still piles patches into the viewer,
        // and those patches point at real source that an accept would rewrite.
        if (!diffEnabled)
        {
            return (null, null);
        }

        var result = await AddInline(BuildPatch());
        if (result != InlineResult.NoViewerFound)
        {
            return (null, null);
        }

        return (
            "No DiffEngineViewer was found, so the snapshot could not be opened for review. Install it: dotnet tool install -g DiffEngineViewer",
            await WriteStaging());
    }

    /// <summary>
    /// Fallback for when no viewer can be resolved, such as a net462 consumer or a RID with no
    /// native renderer. Writes the received and expected text so there is still something to look
    /// at, and opens whatever diff tool is configured.
    /// </summary>
    async Task<StagedInline?> WriteStaging()
    {
        if (IsBuildServer() ||
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

        nameBuilder.Append(Fnv1a.Hash($"{MappedSourceFile}:{inline.Line}"));
        nameBuilder.Append('.');
        nameBuilder.Append(Namer.RuntimeAndVersion);
        var baseName = nameBuilder.ToString();
        var received = Path.Combine(stagingDirectory, $"{baseName}.received.txt");
        // Named .expected (not .verified) so *.verified.* tooling globs never see it
        var expected = Path.Combine(stagingDirectory, $"{baseName}.expected.txt");
        var patch = Path.Combine(stagingDirectory, $"{baseName}.inlinepatch");

        IoHelpers.WriteText(received, new(Rendered));
        await VerifierSettings.RunAddTestAttachment(received);
        IoHelpers.WriteText(expected, new(NormalizedExpected ?? ""));
        // Kept so the snapshot can still be reviewed by hand:
        // DiffEngineViewer --inline --source <cs> --line <n> < thisFile
        InlinePatchFile.Write(patch, BuildPatch());

        if (diffEnabled)
        {
            await DiffRunner.LaunchForTextAsync(received, expected, VerifierSettings.Encoding);
        }

        return new(received, expected, patch);
    }

    InlinePatch BuildPatch() =>
        new(
            MappedSourceFile,
            inline.Line,
            inline.Expression,
            Rendered,
            inline.Mode);

    /// <summary>
    /// Strips the Snapshot call, for when a test that had an inline literal is moving back to a
    /// .verified file. Nothing is queued for review: the file snapshot is what gets accepted.
    /// </summary>
    public static bool TryRemove(InlineInfo inline)
    {
        if (IsBuildServer())
        {
            return false;
        }

        var patch = new InlinePatch(
            InnerVerifier.MapSourceFile(inline.File),
            inline.Line,
            inline.Expression,
            "",
            InlinePatchMode.Remove);
        return InlineApplier.Apply(patch).Status == InlineApplyStatus.Applied;
    }
}

record StagedInline(string Received, string Expected, string Patch);
