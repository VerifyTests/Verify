// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable UseAwaitUsing

namespace VerifyTests;

[DebuggerDisplay("new = {new.Count} | notEquals = {notEquals.Count} | equal = {equal.Count} | delete = {delete.Count}")]
class VerifyEngine(
    string directory,
    VerifySettings settings,
    IEnumerable<string> verifiedFiles,
    GetFileNames getFileNames,
    GetIndexedFileNames getIndexedFileNames,
    string? typeName,
    string? methodName,
    InlineEngine? inlineEngine = null,
    string? migratedExpected = null)
{
    bool diffEnabled = !DiffRunner.Disabled &&
                       settings.diffEnabled &&
                       !BuildServerDetector.Detected;
    List<NewResult> @new = [];
    List<NotEqualResult> notEquals = [];
    List<FilePair> equal = [];
    List<FilePair> autoVerified = [];
    HashSet<string> delete = [with(verifiedFiles, StringComparer.InvariantCultureIgnoreCase)];

    public IReadOnlyList<FilePair> Equal => equal;
    public IReadOnlyList<FilePair> AutoVerified => autoVerified;

    static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, Target target, bool textHasFailed, bool bypassComparers)
    {
        try
        {
            if (target.TryGetStringBuilder(out var value))
            {
                return await Comparer.Text(file, value, settings, bypassComparers);
            }

            using var stream = target.StreamData;
            stream.MoveToStart();
            return await FileComparer.DoCompare(settings, file, textHasFailed || bypassComparers, stream);
        }
        catch (VerifiedLineEndingException)
        {
            // Already names the file and the fix. Wrapping it hides that behind a generic
            // "failed to compare" message.
            throw;
        }
        catch (Exception exception)
        {
            throw new(
                $"""
                 Failed to compare files:
                 ReceivedPath: {file.ReceivedPath}
                 VerifiedPath: {file.VerifiedPath}
                 """,
                exception);
        }
    }

    public async Task HandleResults(List<Target> targetList)
    {
        if (targetList.Count == 1)
        {
            var target = targetList[0];
            if (inlineEngine is not null)
            {
                await inlineEngine.Compare(target);
                return;
            }

            var file = getFileNames(target);
            SeedMigrated(file);
            Track(target, file);
            var result = await GetResult(settings, file, target, false, false);
            HandleCompareResult(result, file);
            return;
        }

        var planned = Plan(targetList);
        var results = new EqualityResult[planned.Count];
        var textHasFailed = false;
        var bypassComparers = false;

        // A target that differed tells the ones compared after it. The inlined target is compared
        // by its own engine, so it has to feed the same cascade by hand. Without that a first
        // target that differed told the targets after it nothing, and the derived ones kept
        // trusting comparers that exist to tolerate differences the source target had just failed
        // on - which is what BypassComparersForSubsequentOnDifference is for, and it stopped
        // working as soon as that target was the inlined one
        void NoteDifference(in Target target, bool isText)
        {
            if (isText)
            {
                textHasFailed = true;
            }

            if (target.BypassComparersForSubsequentOnDifference)
            {
                bypassComparers = true;
            }

            if (target is { IsSource: true, Conversion: { } conversion })
            {
                differed.Add(conversion);
            }
        }

        foreach (var index in CompareOrder(planned))
        {
            var (target, planFile, isFirst) = planned[index];
            if (planFile is not { } file)
            {
                await inlineEngine!.Compare(target);
                if (inlineEngine.Equality != Equality.Equal)
                {
                    // Always text: Compare refuses a stream outright
                    NoteDifference(target, true);
                }

                continue;
            }

            if (isFirst)
            {
                SeedMigrated(file);
            }

            Track(target, file);

            // A text target that failed has always made the binary targets after it skip their
            // comparers: with nothing else to go on, it is the sign that what they were made from
            // has changed. A target whose source was compared has something better to go on, and
            // a source that passed says its pages are the pages they were. Without this, an info
            // file that changed shape had every page of an unchanged document compared exactly.
            var textFailed = textHasFailed && !SourceCompared(target);
            var result = await GetResult(settings, file, target, textFailed, bypassComparers || SourceDiffered(target));
            if (result.Equality != Equality.Equal)
            {
                NoteDifference(target, file.IsText);
            }

            results[index] = result;
        }

        // In the order planned rather than the order compared, so the callbacks and the exception
        // message list the files as they always have
        for (var index = 0; index < planned.Count; index++)
        {
            if (planned[index].File is { } file)
            {
                HandleCompareResult(results[index], file);
            }
        }
    }

    /// <summary>
    /// A target and the file it is compared as. No file for the target that is inlined.
    /// </summary>
    readonly record struct Planned(Target Target, FilePair? File, bool IsFirst);

    /// <summary>
    /// Every target with the file it is compared as. Decided for all of them before any is
    /// compared, so that the order they are compared in is free to differ from the order that
    /// names them.
    /// </summary>
    List<Planned> Plan(List<Target> targetList)
    {
        var planned = new List<Planned>(targetList.Count);

        // Grouped over the full list, including the inlined target, so the file names of the
        // remaining targets are the same whether or not inline is on. That leaves a deliberate
        // gap where the inlined target's #00 would have been.
        var indexed = targetList
            .Select((target, position) => (target, position))
            .ToList();
        foreach (var group in indexed.GroupBy(_ => _.target, targetNameExtensionComparer))
        {
            var targets = group.ToList();
            // Several targets sharing a name and extension are told apart by an index; one on its
            // own keeps the plain name
            var indexNames = targets.Count > 1;
            for (var index = 0; index < targets.Count; index++)
            {
                var (target, position) = targets[index];
                if (position == 0 && inlineEngine is not null)
                {
                    planned.Add(new(target, null, true));
                    continue;
                }

                var file = indexNames
                    ? getIndexedFileNames(target, index.ToString("D2"))
                    : getFileNames(target);
                planned.Add(new(target, file, position == 0));
            }
        }

        return planned;
    }

    /// <summary>
    /// The order the planned targets are compared in: the inlined one, then the sources, then the
    /// rest, each as planned.
    /// </summary>
    /// <remarks>
    /// A source is compared ahead of what was derived from it, because how the derived targets
    /// are compared depends on what its comparison found (<see cref="SourceDiffered" />). Planned
    /// order alone does not give that: an info file is the first target and its document the
    /// second. Outermost first among the sources, since a source rendered from another answers to
    /// that one the same way.
    /// </remarks>
    static IEnumerable<int> CompareOrder(List<Planned> planned)
    {
        var sources = new List<int>();
        var rest = new List<int>();
        for (var index = 0; index < planned.Count; index++)
        {
            var item = planned[index];
            if (item.File is null)
            {
                yield return index;
            }
            else if (item.Target.IsSource)
            {
                sources.Add(index);
            }
            else
            {
                rest.Add(index);
            }
        }

        // A stable sort, so sources as deep as each other stay as planned
        foreach (var index in sources.OrderBy(_ => Depth(planned[_].Target.Conversion)))
        {
            yield return index;
        }

        foreach (var index in rest)
        {
            yield return index;
        }
    }

    static int Depth(ConversionToken? conversion)
    {
        var depth = 0;
        while (conversion is not null)
        {
            depth++;
            conversion = conversion.Parent;
        }

        return depth;
    }

    // The conversion each compared file came out of, by received path, and the file the source of
    // each conversion was compared as. Together, what ties a file to the document it was derived
    // from once the targets themselves have gone
    Dictionary<string, Derivation> derivations = [];
    Dictionary<ConversionToken, SourceFile> sources = [];

    // The conversions whose source was new or not equal
    HashSet<ConversionToken> differed = [];

    readonly record struct Derivation(ConversionToken Conversion, bool IsSource);

    readonly record struct SourceFile(FilePair File, bool IsNamed);

    void Track(in Target target, in FilePair file)
    {
        if (target.Conversion is not { } conversion)
        {
            return;
        }

        derivations[file.ReceivedPath] = new(conversion, target.IsSource);
        if (target.IsSource)
        {
            sources[conversion] = new(file, target.Name is not null);
        }
    }

    /// <summary>
    /// Whether a source this target was derived from differed, in which case the target skips its
    /// registered comparer and is compared exactly. A comparer exists to tolerate differences, and
    /// a document that has changed is the one case where a page of it should not be given the
    /// benefit of the doubt.
    /// </summary>
    /// <remarks>
    /// Only the sources above the target. One conversion's document differing says nothing of the
    /// pages of another's, which is what the flag this replaces,
    /// <see cref="Target.BypassComparersForSubsequentOnDifference" />, could not express.
    /// </remarks>
    /// <summary>
    /// Whether a source this target was derived from is itself a target of the verification, and
    /// so has been compared: sources are compared ahead of everything derived from them.
    /// </summary>
    bool SourceCompared(in Target target)
    {
        if (target.IsSource)
        {
            return false;
        }

        var conversion = target.Conversion;
        while (conversion is not null)
        {
            if (sources.ContainsKey(conversion))
            {
                return true;
            }

            conversion = conversion.Parent;
        }

        return false;
    }

    bool SourceDiffered(in Target target)
    {
        var conversion = target.Conversion;
        // A source answers to the sources above it, not to itself
        if (target.IsSource)
        {
            conversion = conversion?.Parent;
        }

        while (conversion is not null)
        {
            if (differed.Contains(conversion))
            {
                return true;
            }

            conversion = conversion.Parent;
        }

        return false;
    }

    /// <summary>
    /// A verification that just migrated away from an inline snapshot has no verified file yet,
    /// but its literal was the approved content, so that becomes the file. Without this the
    /// migration reads as a brand new snapshot and the approved text is lost.
    /// </summary>
    void SeedMigrated(in FilePair file)
    {
        if (migratedExpected is null ||
            File.Exists(file.VerifiedPath))
        {
            return;
        }

        IoHelpers.WriteText(file.VerifiedPath, new(migratedExpected));
    }

    void HandleCompareResult(EqualityResult result, FilePair file)
    {
        switch (result.Equality)
        {
            case Equality.New:
                AddMissing(new(file, result.ReceivedText));
                break;
            case Equality.NotEqual:
                AddNotEquals(new(file, result.Message, result.ReceivedText, result.VerifiedText));
                break;
            case Equality.Equal:
                AddEquals(file);
                break;
        }
    }

    void AddMissing(in NewResult item)
    {
        @new.Add(item);
        delete.Remove(item.File.VerifiedPath);
    }

    void AddNotEquals(in NotEqualResult notEqual)
    {
        notEquals.Add(notEqual);
        delete.Remove(notEqual.File.VerifiedPath);
    }

    void AddEquals(in FilePair item)
    {
        delete.Remove(item.VerifiedPath);
        equal.Add(item);
    }

    public async Task ThrowIfRequired()
    {
        ProcessEquals();
        SettleRaisedDeletes();

        var inlineFailed = false;
        if (inlineEngine != null)
        {
            if (inlineEngine.Equality == Equality.Equal)
            {
                inlineEngine.Settle();
            }
            else
            {
                inlineFailed = true;
            }
        }

        if (!inlineFailed &&
            @new.Count == 0 &&
            notEquals.Count == 0 &&
            delete.Count == 0)
        {
            return;
        }

        bool allDeletesVerified;
        bool allNewVerified;
        bool allNotEqualsVerified;
        try
        {
            allDeletesVerified = await ProcessDeletes();

            allNewVerified = await ProcessNew();

            allNotEqualsVerified = await ProcessNotEquals();
        }
        finally
        {
            // Whatever the three left pending, including when a callback of one threw part way
            await Report();
        }

        var (allInlineVerified, inlineSection) = await ProcessInline(inlineFailed);

        var throwException = VerifierSettings.throwException || settings.throwException;

        if (allDeletesVerified &&
            allNewVerified &&
            allNotEqualsVerified &&
            allInlineVerified &&
            !throwException)
        {
            return;
        }

        var message = VerifyExceptionMessageBuilder.Build(
            directory,
            @new,
            notEquals,
            delete,
            equal,
            inlineSection,
            inlineHint);
        throw new VerifyException(message);
    }

    string? inlineHint;

    async Task<(bool verified, InlineSection? section)> ProcessInline(bool inlineFailed)
    {
        if (!inlineFailed)
        {
            return (true, null);
        }

        var engine = inlineEngine!;
        StagedInline? staged = null;
        // The source file stands in for the verified file, because that is where the snapshot lives
        var verified = IsAutoVerify(engine.MappedSourceFile) && engine.TryApply();
        if (!verified)
        {
            (inlineHint, staged) = await engine.Queue();
        }

        return (
            verified,
            new(
                engine.MappedSourceFile,
                engine.Line,
                engine.Equality == Equality.New,
                engine.Rendered,
                engine.NormalizedExpected,
                staged));
    }

    internal bool IsAutoVerify(string verifiedFile)
    {
        // The global delegate needs the type/method name; the per-settings
        // delegate does not, so it must not be gated on typeName (which is null
        // for the file-level InnerVerifier(directory, name) API).
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

    async Task<bool> ProcessDeletes()
    {
        var verified = true;
        foreach (var item in delete)
        {
            if (!await ProcessDeletes(item))
            {
                verified = false;
            }
        }

        return verified;
    }

    async Task<bool> ProcessDeletes(string file)
    {
        var autoVerify = IsAutoVerify(file);
        await settings.RunOnVerifyDelete(file, autoVerify);

        if (autoVerify)
        {
            File.Delete(file);
            return true;
        }

        pendingDeletes.Add(file);

        return false;
    }

    /// <summary>
    /// Every verified file this verification compared against is in use, whatever the comparison
    /// found, so a delete an earlier run raised for one of them no longer describes a stale file.
    /// </summary>
    void SettleRaisedDeletes()
    {
        foreach (var item in equal)
        {
            RaisedDeletes.SettleIfRaised(item.VerifiedPath);
        }

        foreach (var item in notEquals)
        {
            RaisedDeletes.SettleIfRaised(item.File.VerifiedPath);
        }

        foreach (var item in @new)
        {
            RaisedDeletes.SettleIfRaised(item.File.VerifiedPath);
        }
    }

    async Task<bool> ProcessNotEquals()
    {
        var verified = true;
        foreach (var notEqual in notEquals)
        {
            await VerifierSettings.RunAddTestAttachment(notEqual.File.ReceivedPath);
            var autoVerify = IsAutoVerify(notEqual.File.VerifiedPath);
            await settings.RunOnVerifyMismatch(notEqual.File, notEqual.Message, autoVerify);
            if (!RunDiffAutoCheck(notEqual.File, autoVerify))
            {
                verified = false;
            }
        }

        return verified;
    }

    void ProcessEquals()
    {
        if (!diffEnabled)
        {
            return;
        }

        foreach (var item in equal)
        {
            DiffRunner.Kill(item.ReceivedPath, item.VerifiedPath);
        }
    }

    bool RunDiffAutoCheck(FilePair file, bool autoVerify)
    {
        if (autoVerify)
        {
            autoVerified.Add(file);
            AcceptChanges(file);
            return true;
        }

        // The received file is being left on disk. It is reported once every pending file is
        // known: see Report
        pendingMoves.Add(file);

        return false;
    }

    // The received files left on disk, and the verified files no target produced, that were not
    // auto verified: what Report tells DiffEngine of
    List<FilePair> pendingMoves = [];
    List<string> pendingDeletes = [];

    /// <summary>
    /// Swapped in tests. What reaches a diff tool is otherwise only observable from one.
    /// The second argument is the received file of the source the pair was derived from, or null.
    /// </summary>
    internal static Func<FilePair, string?, Task> LaunchDiff = DefaultLaunchDiff;

    static Task DefaultLaunchDiff(FilePair file, string? source)
    {
        var encoding = VerifierSettings.Encoding;
        if (source is null)
        {
            if (file.IsText)
            {
                return DiffRunner.LaunchForTextAsync(file.ReceivedPath, file.VerifiedPath, encoding);
            }

            return DiffRunner.LaunchAsync(file.ReceivedPath, file.VerifiedPath, encoding);
        }

        if (file.IsText)
        {
            return DiffRunner.LaunchDerivedForTextAsync(file.ReceivedPath, file.VerifiedPath, source, encoding);
        }

        return DiffRunner.LaunchDerivedAsync(file.ReceivedPath, file.VerifiedPath, source, encoding);
    }

    /// <summary>
    /// Tells DiffEngine, and the received maps, of everything this verification left pending.
    /// </summary>
    /// <remarks>
    /// Once, after the last of it is known, rather than file by file as each is processed. A file
    /// derived from a document says so only while the document is itself pending, and DiffEngine
    /// has to hear of the document before it hears of anything derived from it, so that a tool
    /// drawing the document can show its pages beneath it rather than each in a window of its own.
    /// Neither can be known while the files are still being gone through: the document is the
    /// second target more often than the first.
    /// <para>
    /// Deletes that stand alone stay ahead of the moves, as they always were. A delete derived
    /// from a document follows the launch for that document.
    /// </para>
    /// </remarks>
    async Task Report()
    {
        var pending = new HashSet<string>(pendingMoves.Select(_ => _.ReceivedPath));

        var moves = new List<(FilePair file, string? source)>(pendingMoves.Count);
        foreach (var file in pendingMoves)
        {
            moves.Add((file, SourceOf(file, pending)));
        }

        var deletes = new List<(string file, string? source)>(pendingDeletes.Count);
        foreach (var file in pendingDeletes)
        {
            deletes.Add((file, SourceOfStale(file, pending)));
        }

        foreach (var (file, source) in deletes)
        {
            if (source is null)
            {
                await RaisedDeletes.Raise(file, null);
            }
        }

        foreach (var (file, source) in moves)
        {
            if (source is null)
            {
                await ReportMove(file, null);
            }
        }

        foreach (var (file, source) in moves)
        {
            if (source is not null)
            {
                await ReportMove(file, source);
            }
        }

        foreach (var (file, source) in deletes)
        {
            if (source is not null)
            {
                await RaisedDeletes.Raise(file, ForDiffEngine(source));
            }
        }
    }

    Task ReportMove(FilePair file, string? source)
    {
        // The received file is being left on disk, so record the verified file it belongs to.
        // With its source whether or not a diff tool is told: the map is for tooling that finds
        // the files on disk, where the source is as much there as the file is
        ReceivedMap.Write(file, source);

        if (diffEnabled)
        {
            return LaunchDiff(file, source);
        }

        return Task.CompletedTask;
    }

    // Nothing was launched for a source while diff is off, so there is nothing for DiffEngine to
    // show a derived file beneath
    string? ForDiffEngine(string source)
    {
        if (diffEnabled)
        {
            return source;
        }

        return null;
    }

    /// <summary>
    /// The received file of the source a pending file was derived from, or null for a file that
    /// stands alone: one no conversion derived, the source itself, or a file whose source passed
    /// or was auto verified and so has no received file to name.
    /// </summary>
    string? SourceOf(in FilePair file, HashSet<string> pending)
    {
        if (!derivations.TryGetValue(file.ReceivedPath, out var derivation))
        {
            return null;
        }

        // A source is derived from the sources above it, not from itself
        if (derivation.IsSource)
        {
            return OutermostPending(derivation.Conversion.Parent, pending);
        }

        return OutermostPending(derivation.Conversion, pending);
    }

    /// <summary>
    /// The outermost source, of this conversion and those it came out of, that is pending.
    /// </summary>
    /// <remarks>
    /// One level is all a diff tool is told: a page of a pdf that was rendered from a docx is a
    /// file of the docx, as the pdf is. The outermost because that is the document the test
    /// verified, and so the one a reviewer is looking at.
    /// </remarks>
    string? OutermostPending(ConversionToken? conversion, HashSet<string> pending)
    {
        string? source = null;
        while (conversion is not null)
        {
            if (sources.TryGetValue(conversion, out var candidate) &&
                pending.Contains(candidate.File.ReceivedPath))
            {
                source = candidate.File.ReceivedPath;
            }

            conversion = conversion.Parent;
        }

        return source;
    }

    /// <summary>
    /// The source a verified file that no target produced is taken to have been derived from: a
    /// page a document no longer has.
    /// </summary>
    /// <remarks>
    /// There is no target to say, so it goes by the name: the file of a pending source that was
    /// named when its own name carries on from that one, and otherwise the one pending source
    /// that was not named, where there is exactly one. Anything else stands alone, which costs a
    /// reviewer one more accept and never a wrong one.
    /// </remarks>
    string? SourceOfStale(string verifiedFile, HashSet<string> pending)
    {
        ConversionToken? named = null;
        var namedLength = 0;
        var unnamed = new HashSet<string>();
        foreach (var (conversion, source) in sources)
        {
            if (!pending.Contains(source.File.ReceivedPath))
            {
                continue;
            }

            if (!source.IsNamed)
            {
                // Sources rendered from one another are one document to a reviewer
                unnamed.Add(OutermostPending(conversion, pending)!);
                continue;
            }

            var stem = Stem(source.File);
            if (stem.Length > namedLength &&
                verifiedFile.StartsWith($"{stem}.", StringComparison.OrdinalIgnoreCase))
            {
                named = conversion;
                namedLength = stem.Length;
            }
        }

        if (named is not null)
        {
            return OutermostPending(named, pending);
        }

        if (unnamed.Count == 1)
        {
            return unnamed.Single();
        }

        return null;
    }

    // A verified path less what follows the name: ".verified.pdf", or ".pdf" in a directory of
    // verified files
    static string Stem(in FilePair file)
    {
        var path = file.VerifiedPath;
        var suffix = $".verified.{file.Extension}";
        if (path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return path[..^suffix.Length];
        }

        return path[..^(file.Extension.Length + 1)];
    }

    async Task<bool> ProcessNew()
    {
        var verified = true;
        foreach (var file in @new)
        {
            await VerifierSettings.RunAddTestAttachment(file.File.ReceivedPath);
            var autoVerify = IsAutoVerify(file.File.VerifiedPath);
            await settings.RunOnFirstVerify(file, autoVerify);
            if (!RunDiffAutoCheck(file.File, autoVerify))
            {
                verified = false;
            }
        }

        return verified;
    }

    static void AcceptChanges(in FilePair file) =>
        File.Move(file.ReceivedPath, file.VerifiedPath, true);

    static readonly IEqualityComparer<Target> targetNameExtensionComparer =
        EqualityComparer<Target>.Create(_ => (_.Name ?? "", _.Extension));
}
