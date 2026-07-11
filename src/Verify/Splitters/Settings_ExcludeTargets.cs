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
        context.ContainsKey(VerifySettings.excludedTargetsKey);

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
}
