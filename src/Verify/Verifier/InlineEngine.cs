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

    /// <summary>
    /// The verdict, which only exists once <see cref="Compare" /> has reached one. Held as a
    /// nullable rather than left to default, because the default of this enum is Equal: a
    /// verification that never compared anything would otherwise report that it passed, and
    /// nothing downstream could tell that apart from one that did.
    /// </summary>
    public Equality Equality =>
        equality ?? throw new("The inline snapshot was never compared.");

    Equality? equality;
    public string Rendered { get; private set; } = null!;
    public string? NormalizedExpected { get; private set; }

    /// <summary>
    /// The snapshot the source file holds as it stands, which is what a patch anchors to. Kept
    /// apart from <see cref="NormalizedExpected"/>, which the trailing newline tolerance may have
    /// shortened: an anchor that does not match the source finds nothing.
    /// </summary>
    string? SnapshotInSource { get; set; }

    public async Task Compare(Target target)
    {
        if (target.IsStream)
        {
            throw new VerifyException(
                $"""
                 Inline snapshots only support text content. The first target, with extension '{target.Extension}', is a binary stream.
                 Remove the `Snapshot(...)` call, or add `NotInline()`, to keep this test on verified files. Only an explicit `Snapshot(...)` reaches this: the global switch declines a binary target and falls back to files.
                 """);
        }

        target.TryGetStringBuilder(out var builder);
        Rendered = builder!.ToString();

        if (inline.Expected is null)
        {
            equality = Equality.New;
            return;
        }

        // What the source is holding right now, which is also what a patch is anchored to
        SnapshotInSource = NormalizeExpected(inline.Expected, inline.File);
        NormalizedExpected = Comparer.ApplyTrailingNewlineTolerance(SnapshotInSource, Rendered.Length);

        // Through the comparison the file pipeline uses, rather than an ordinal compare that
        // looked like it. A suite with a string comparer registered - one that tolerates an
        // ordering, or a timestamp, or a rounding - passed against its verified files and started
        // failing the moment the same snapshot moved inline, with nothing saying why
        var result = await Comparer.CompareStrings(target.Extension, builder, NormalizedExpected, settings, false);
        equality = result.IsEqual
            ? Equality.Equal
            : Equality.NotEqual;
    }

    /// <summary>
    /// The snapshot an expected argument holds. The literal inherits the source file's line
    /// endings, so mirror Comparer.Text normalization; and it inherits its language, so ask that
    /// language what the value it produced actually means.
    /// <para>
    /// The second half is only ever a no-op for C#, whose compiler takes the layout off a raw
    /// string itself. F# has no such form, so the value still carries the line break after the
    /// opening delimiter and the indentation of every line, and comparing it as it stands would
    /// fail every F# snapshot against itself. Also used when a literal is migrated into a verified
    /// file, so the two can never drift.
    /// </para>
    /// </summary>
    internal static string NormalizeExpected(string expected, string sourceFile) =>
        SourceLanguage.ForFile(sourceFile)
            .SnapshotValue(expected)
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');

    /// <summary>
    /// Drops anything a prior failing run left queued in the viewer.
    /// </summary>
    public void Settle()
    {
        if (diffEnabled)
        {
            // The member goes with the line, because the line alone stops finding the entry as
            // soon as an accept earlier in the file inserts a literal above this call site.
            DiffRunner.SettleInline(MappedSourceFile, inline.Line, inline.MemberName);
            ClearStaged(MappedSourceFile, inline.Line, inline.MemberName);
        }
    }

    /// <summary>
    /// A settle only reaches a queue owner, and a snapshot can be on disk instead: staged by a run
    /// that found no owner, or written out by one on its way out. Those files are what accept
    /// tooling reads, so without this the snapshot stays pending for a test that now passes.
    /// </summary>
    static void ClearStaged(string mappedSourceFile, int line, string? memberName) =>
        InlineStaging.Clear(mappedSourceFile, line, memberName, VerifierSettings.IntermediateDir);

    /// <summary>
    /// The members this process has actually inlined a verification for, so a retire in the same
    /// member does not reach for one of their entries. See <see cref="Retire" />.
    /// </summary>
    static readonly ConcurrentDictionary<string, byte> inlinedMembers = new(StringComparer.OrdinalIgnoreCase);

    public static void RecordInline(string mappedSourceFile, string? memberName)
    {
        if (memberName is not null)
        {
            inlinedMembers[$"{mappedSourceFile}|{memberName}"] = 0;
        }
    }

    /// <summary>
    /// Drops whatever a previous run queued for a call site that is no longer an inline snapshot:
    /// <c>NotInline</c>, a global switch that declined it, or a literal that outgrew the size
    /// limit. Nothing here compares anything, so this is static and needs no engine — there is no
    /// inline verification to build one for, which is the whole point.
    /// </summary>
    /// <remarks>
    /// Without this the entry outlives the decision that made it meaningless. Settling only ever
    /// happened on the inline path, so a verification that stopped being inline left its queued
    /// snapshot pending for good: no run would ever settle it, and the reviewer was left with an
    /// entry for a test that passes.
    /// <para>
    /// The member is withheld once this process has inlined something in that member. It is the
    /// owner's fallback for a call site whose line has moved, and it cannot tell that from a
    /// sibling call site in the same member — so a member holding both an inline verification and
    /// a declined one would have the inline one's freshly queued entry retired out from under it,
    /// every run, and its snapshot would never be reviewable. Withholding the member costs only
    /// the drift tolerance, and only for members that have an inline verification to lose.
    /// </para>
    /// </remarks>
    public static void Retire(VerifySettings settings, string? sourceFile, int line, string? memberName)
    {
        if (DiffRunner.Disabled ||
            !settings.diffEnabled ||
            IsBuildServer())
        {
            return;
        }

        if (sourceFile is null ||
            line == 0 ||
            !InlineInfo.IsSupported(sourceFile))
        {
            return;
        }

        var mapped = InnerVerifier.MapSourceFile(sourceFile);
        var member = memberName is not null &&
                     inlinedMembers.ContainsKey($"{mapped}|{memberName}")
            ? null
            : memberName;
        DiffRunner.RetireInline(mapped, line, member);
        ClearStaged(mapped, line, member);
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
            $"No DiffEngineViewer was found, so the snapshot could not be opened for review. Install it: dotnet tool install -g {ViewerPackage}",
            await WriteStaging());
    }

    /// <summary>
    /// The viewer ships one package per platform, and there is no DiffEngineViewer package: the
    /// command the hint used to give failed with "not found" for everyone who ran it.
    /// </summary>
    static string ViewerPackage
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "DiffEngineViewer.Windows";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "DiffEngineViewer.Mac";
            }

            return "DiffEngineViewer.Linux";
        }
    }

    /// <summary>
    /// Fallback for when no viewer can be resolved, such as a net462 consumer or a RID with no
    /// native renderer. Writes the received and expected text so there is still something to look
    /// at, and opens whatever diff tool is configured.
    /// </summary>
    async Task<StagedInline?> WriteStaging()
    {
        // No build server check: the only caller is Queue, which returns before reaching here
        // unless diffEnabled, and that already required this not to be one
        if (VerifierSettings.IntermediateDir is null)
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

    /// <summary>
    /// What the viewer labels and groups a queued entry by. Type and method, the pair that already
    /// names a verified file, so the queue shows a name the reader knows from test output instead
    /// of falling back to the call site. Null only when neither could be resolved, which is the
    /// explicit Snapshot path outside a recognised test.
    /// </summary>
    string? TestName => (typeName, methodName) switch
    {
        (not null, not null) => $"{typeName}.{methodName}",
        _ => typeName ?? methodName
    };

    InlinePatch BuildPatch() =>
        new(
            MappedSourceFile,
            inline.Line,
            inline.Expression,
            Rendered,
            inline.Mode)
        {
            TestName = TestName,
            // The anchors that say which call this patch came from. The expression is what the
            // source says and is null from F#, whose compiler does not implement the attribute;
            // the value is what it means, and the member is where it lives
            OriginalValue = SnapshotInSource,
            MemberName = inline.MemberName,
            // What the accept may chain onto beyond the built-in entry points. Carried on the
            // patch rather than known to the applier, since it is the test project that declares
            // its own wrappers and the patch is what crosses to the process that applies it
            EntryPoints = VerifierSettings.inlineEntryPoints
        };

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
            InlinePatchMode.Remove)
        {
            // Applied here rather than queued, so it is never displayed and has no name to show.
            TestName = null,
            OriginalValue = inline.Expected is null ? null : NormalizeExpected(inline.Expected, inline.File),
            MemberName = inline.MemberName
        };
        return InlineApplier.Apply(patch).Status == InlineApplyStatus.Applied;
    }
}

record StagedInline(string Received, string Expected, string Patch);
