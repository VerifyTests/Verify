namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static HashSet<string>? excludedTargets;

    /// <summary>
    /// Excludes every target with one of <paramref name="extensions" /> from all verifications.
    /// Any existing verified file for an excluded extension is treated as pending deletion.
    /// Intended for converters that emit a source document (eg <c>pdf</c> or <c>docx</c>) alongside
    /// the info file and rendered pages, where committing the source document is not wanted.
    /// </summary>
    public static void ExcludeTargets(params string[] extensions)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        excludedTargets = AddExtensions(excludedTargets, extensions);
    }

    /// <summary>
    /// Whether a target with <paramref name="extension" /> will be excluded from the current
    /// verification, via either the global or the per-verification <c>ExcludeTargets</c>.
    /// A converter can call this on its <c>context</c> to skip producing an expensive target
    /// (eg rendering a document) that would otherwise be dropped.
    /// </summary>
    public static bool IsTargetExcluded(this IReadOnlyDictionary<string, object> context, string extension)
    {
        Guards.AgainstBadExtension(extension);
        return IsExcluded(context, extension);
    }

    internal static bool IsExcluded(IReadOnlyDictionary<string, object> context, string extension) =>
        excludedTargets?.Contains(extension) == true ||
        (context.TryGetValue(VerifySettings.excludedTargetsKey, out var value) &&
         ((HashSet<string>) value).Contains(extension));

    internal static bool AnyExcludedTargets(IReadOnlyDictionary<string, object> context) =>
        excludedTargets is { Count: > 0 } ||
        excludedDerivedTargets is { Count: > 0 } ||
        context.ContainsKey(VerifySettings.excludedTargetsKey) ||
        context.ContainsKey(VerifySettings.excludedDerivedTargetsKey);

    internal static HashSet<string>? excludedDerivedTargets;

    /// <summary>
    /// Excludes, from all verifications, every target with one of <paramref name="extensions" />
    /// that a converter derived from a document: a rendered image or the text of a page, a csv of
    /// a sheet. The document itself, and a target passed to a verification directly, are kept,
    /// which is the difference from <see cref="ExcludeTargets" />: excluding <c>png</c> there
    /// would also exclude a verified image.
    /// Any existing verified file for an excluded derived target is treated as pending deletion.
    /// </summary>
    public static void ExcludeDerivedTargets(params string[] extensions)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        excludedDerivedTargets = AddExtensions(excludedDerivedTargets, extensions);
    }

    /// <summary>
    /// Whether a derived target with <paramref name="extension" /> will be excluded from the
    /// current verification, via <c>ExcludeDerivedTargets</c> or <c>ExcludeTargets</c>, global or
    /// per-verification. A converter can call this on its <c>context</c> to skip producing an
    /// expensive derived target (eg rendering each page) that would otherwise be dropped.
    /// </summary>
    public static bool IsDerivedTargetExcluded(this IReadOnlyDictionary<string, object> context, string extension)
    {
        Guards.AgainstBadExtension(extension);
        return IsDerivedExcluded(context, extension);
    }

    internal static bool IsDerivedExcluded(IReadOnlyDictionary<string, object> context, string extension) =>
        IsExcluded(context, extension) ||
        excludedDerivedTargets?.Contains(extension) == true ||
        (context.TryGetValue(VerifySettings.excludedDerivedTargetsKey, out var value) &&
         ((HashSet<string>) value).Contains(extension));

    // Builds a fresh set rather than mutating existing, so a set shared with cloned settings
    // (Context values are copied by reference) is never mutated in place.
    internal static HashSet<string> AddExtensions(HashSet<string>? existing, string[] extensions)
    {
        if (extensions.Length == 0)
        {
            throw new ArgumentException("At least one extension is required.", nameof(extensions));
        }

        var result = existing == null ?
            new(StringComparer.OrdinalIgnoreCase) :
            new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
        foreach (var extension in extensions)
        {
            Guards.AgainstBadExtension(extension);
            result.Add(extension);
        }

        return result;
    }
}

public partial class VerifySettings
{
    // The per-verification excluded set lives in Context so that a converter, which receives
    // Context, can consult it via IsTargetExcluded.
    internal const string excludedTargetsKey = "Verify.ExcludeTargets";

    /// <summary>
    /// Excludes every target with one of <paramref name="extensions" /> from the current verification.
    /// Any existing verified file for an excluded extension is treated as pending deletion.
    /// Intended for converters that emit a source document (eg <c>pdf</c> or <c>docx</c>) alongside
    /// the info file and rendered pages, where committing the source document is not wanted.
    /// </summary>
    public void ExcludeTargets(params string[] extensions)
    {
        var existing = Context.TryGetValue(excludedTargetsKey, out var value) ? (HashSet<string>) value : null;
        Context[excludedTargetsKey] = VerifierSettings.AddExtensions(existing, extensions);
    }

    internal const string excludedDerivedTargetsKey = "Verify.ExcludeDerivedTargets";

    /// <summary>
    /// Excludes, from the current verification, every target with one of
    /// <paramref name="extensions" /> that a converter derived from a document: a rendered image
    /// or the text of a page, a csv of a sheet. The document itself, and a target passed to the
    /// verification directly, are kept.
    /// Any existing verified file for an excluded derived target is treated as pending deletion.
    /// </summary>
    public void ExcludeDerivedTargets(params string[] extensions)
    {
        var existing = Context.TryGetValue(excludedDerivedTargetsKey, out var value) ? (HashSet<string>) value : null;
        Context[excludedDerivedTargetsKey] = VerifierSettings.AddExtensions(existing, extensions);
    }
}

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.ExcludeTargets" />
    [Pure]
    public SettingsTask ExcludeTargets(params string[] extensions)
    {
        CurrentSettings.ExcludeTargets(extensions);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ExcludeDerivedTargets" />
    [Pure]
    public SettingsTask ExcludeDerivedTargets(params string[] extensions)
    {
        CurrentSettings.ExcludeDerivedTargets(extensions);
        return this;
    }
}
