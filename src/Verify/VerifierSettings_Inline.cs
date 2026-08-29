namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static GlobalInline? inline;
    internal static int? inlineMaxLines;
    internal static bool inlineApplyMaxLinesToExisting;
    internal static string[]? inlineEntryPoints;

    /// <summary>
    /// Use inline snapshots for every verification: the expected content lives in a
    /// <c>.Snapshot(...)</c> call in the test source instead of a <c>.verified</c> file.
    /// Call from a module initializer.
    /// <para>
    /// Only the first target is inlined. Any others are written to files as usual, keeping the
    /// names they would have had, so turning this on never renames a snapshot file.
    /// </para>
    /// <para>
    /// Opt a single test out with <see cref="VerifySettings.NotInline" />, and an extension out
    /// with <see cref="Target.DontInline" />.
    /// </para>
    /// </summary>
    /// <param name="inline">
    /// Decides, per verification, whether to use an inline snapshot. Null means every verification.
    /// </param>
    /// <param name="maxLines">
    /// The most lines a snapshot may have to be inlined. A longer result uses a <c>.verified</c>
    /// file instead. Null means no limit.
    /// <para>
    /// Combines with <paramref name="inline" /> as an and: both have to accept a verification for
    /// it to be inlined.
    /// </para>
    /// <para>
    /// Counts the lines of the snapshot content, not the lines the literal occupies in source. A
    /// raw string literal adds two delimiter lines plus indentation on top.
    /// </para>
    /// </param>
    /// <param name="applyMaxLinesToExisting">
    /// Apply <paramref name="maxLines" /> to verifications that already have a
    /// <c>.Snapshot(...)</c> call. An over the limit one has the call stripped from the source and
    /// moves to a <c>.verified</c> file, seeded with the literal it had. Requires
    /// <paramref name="maxLines" />. Nothing is rewritten on a build server.
    /// </param>
    public static void Inline(
        GlobalInline? inline = null,
        int? maxLines = null,
        bool applyMaxLinesToExisting = false)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();

        if (maxLines < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLines), maxLines, "maxLines must be at least 1.");
        }

        if (maxLines is null &&
            applyMaxLinesToExisting)
        {
            throw new ArgumentException("applyMaxLinesToExisting requires maxLines.", nameof(applyMaxLinesToExisting));
        }

        VerifierSettings.inline = inline ?? ((_, _, _, _) => true);
        inlineMaxLines = maxLines;
        inlineApplyMaxLinesToExisting = applyMaxLinesToExisting;
    }

    /// <summary>
    /// Members that an inline snapshot may be accepted into, beyond the entry points the adapters
    /// expose. Call from a module initializer.
    /// <para>
    /// For a test project that reaches verify through a wrapper of its own. Accepting a new inline
    /// snapshot chains a <c>.Snapshot(...)</c> call onto the call written in the test, so the
    /// wrapper has to return a <see cref="SettingsTask" /> for that to compile, and has to forward
    /// both <c>sourceFile</c> and <c>lineNumber</c> for it to land on the right call. Neither is
    /// readable from the file being accepted into - often not even from the same project - so
    /// naming a wrapper here is the assertion that it does both.
    /// </para>
    /// <para>
    /// A wrapper that is not named keeps its verifications on <c>.verified</c> files, which is the
    /// outcome to prefer over an accept that writes source that does not compile.
    /// </para>
    /// </summary>
    public static void AddInlineEntryPoint(params string[] names)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();

        var combined = inlineEntryPoints is null ? [] : new List<string>(inlineEntryPoints);
        foreach (var name in names)
        {
            // The patcher matches a whole token, so anything else - a receiver, an argument list,
            // a space - can only ever fail to match, and would do it silently
            if (!IsIdentifier(name))
            {
                throw new ArgumentException($"Inline entry points are member names, so must be identifiers. Value: {name}", nameof(names));
            }

            if (!combined.Contains(name, StringComparer.Ordinal))
            {
                combined.Add(name);
            }
        }

        inlineEntryPoints = combined.ToArray();
    }

    static bool IsIdentifier(string value)
    {
        if (value.Length == 0 ||
            char.IsDigit(value[0]))
        {
            return false;
        }

        foreach (var character in value)
        {
            if (character != '_' &&
                !char.IsLetterOrDigit(character))
            {
                return false;
            }
        }

        return true;
    }
}
