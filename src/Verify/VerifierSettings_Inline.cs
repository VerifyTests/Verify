namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static GlobalInline? inline;

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
    public static void Inline() =>
        Inline((_, _, _, _) => true);

    /// <inheritdoc cref="Inline()" />
    /// <param name="inline">Decides, per verification, whether to use an inline snapshot.</param>
    public static void Inline(GlobalInline inline)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.inline = inline;
    }
}
