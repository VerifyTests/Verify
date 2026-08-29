namespace VerifyTests;

/// <summary>
/// Whether the call site a new inline snapshot would be accepted into can host one.
/// <para>
/// The global switch decides inline from what it knows of the verification: the framework, the
/// targets, the size. None of that says anything about the source the accept has to rewrite, and
/// the accept chains a <c>.Snapshot(...)</c> call onto the verify call written in the test, which
/// only compiles where that call returns a <see cref="SettingsTask" />. A verification reached
/// through a wrapper of the test project's own does not, and accepting one wrote source that no
/// longer compiled - with the verified file already deleted as redundant.
/// </para>
/// <para>
/// So the source is asked, through the same locator that does the accept, before the verification
/// declares itself inline. Only for a new snapshot: an existing one is a <c>Snapshot(...)</c> call
/// that is already in the file and already compiling.
/// </para>
/// </summary>
static class InlineAnchor
{
    /// <summary>
    /// Per call site, which is stable for the life of the process and asked once per run of every
    /// test whose snapshot is still new.
    /// </summary>
    static ConcurrentDictionary<(string File, int Line, string? Member), bool> cache = new();

    public static bool CanHost(string sourceFile, int line, string? memberName) =>
        cache.GetOrAdd(
            (InnerVerifier.MapSourceFile(sourceFile), line, memberName),
            static key => Probe(key.File, key.Line, key.Member));

    static bool Probe(string file, int line, string? member)
    {
        var patch = new InlinePatch(file, line, null, "", InlinePatchMode.Append)
        {
            // Nothing is queued or written, so this patch is never displayed and has no name to
            // show. The content is empty for the same reason: nothing is rendered from it
            TestName = null,
            MemberName = member,
            EntryPoints = VerifierSettings.inlineEntryPoints
        };

        // NotFound is the only definitive no. Failed is a source file that could not be read - not
        // deployed, or a build path that maps somewhere this process cannot follow - and taking
        // that for a refusal would quietly drop inline for a whole suite over an unrelated problem
        return InlineApplier.CanAnchor(patch).Status != InlineApplyStatus.NotFound;
    }
}
