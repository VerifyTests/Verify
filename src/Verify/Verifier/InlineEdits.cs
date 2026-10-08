namespace VerifyTests;

/// <summary>
/// The source rewrites this process has made itself, a file at a time, so each later one is asked
/// about the line its call site is on now.
/// <para>
/// A call site's line comes from the compiler, and stands for the file as it was built. An auto
/// verify or a migration away from inline rewrites that file in the middle of the run, and a
/// snapshot is several lines of source, so every call site under the edit has moved by the time
/// its own turn comes. The applier looks near the line it is given: handed the old one, a
/// snapshot whose anchor a neighbour shares was written into the neighbour, an appended one was
/// refused wherever its member had two calls to choose from, and a removal could find the line
/// empty and report the call as already gone.
/// </para>
/// <para>
/// DiffEngine says which lines an applied patch moved, so the edits made here are replayed over
/// each later patch's line, in the order they were made. Only what this process applies itself.
/// A patch that is queued for review keeps the compiler's line, which is what the queue's owner
/// names the entry by and what every other run reports, and an edit made by another process, or
/// by hand since the build, is not known here at all.
/// </para>
/// </summary>
static class InlineEdits
{
    static readonly ConcurrentDictionary<string, List<InlineApplyResult>> files = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Applies a patch this process built, at the line the edits before it left its call site on.
    /// </summary>
    public static InlineApplyResult Apply(InlinePatch patch)
    {
        var edits = files.GetOrAdd(patch.SourceFile, static _ => []);
        // Held across the apply, so the order the edits are kept in is the order they were made
        // in: each says which lines it moved in the file as the ones before it left it
        lock (edits)
        {
            foreach (var edit in edits)
            {
                patch.LineHint = edit.Rebase(patch.LineHint);
            }

            var result = InlineApplier.Apply(patch);
            if (result.MovedBy != 0)
            {
                edits.Add(result);
            }

            return result;
        }
    }
}
