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
        excludedTargets = AddExcludedTargets(excludedTargets, extensions);
    }

    internal static bool AnyExcludedTargets(VerifySettings settings) =>
        excludedTargets != null ||
        settings.excludedTargets != null;

    internal static bool IsTargetExcluded(VerifySettings settings, string extension) =>
        excludedTargets?.Contains(extension) == true ||
        settings.excludedTargets?.Contains(extension) == true;

    internal static HashSet<string> AddExcludedTargets(HashSet<string>? existing, string[] extensions)
    {
        if (extensions.Length == 0)
        {
            throw new ArgumentException("At least one extension is required.", nameof(extensions));
        }

        existing ??= new(StringComparer.OrdinalIgnoreCase);
        foreach (var extension in extensions)
        {
            Guards.AgainstBadExtension(extension);
            existing.Add(extension);
        }

        return existing;
    }
}

public partial class VerifySettings
{
    internal HashSet<string>? excludedTargets;

    /// <summary>
    /// Excludes every target with one of <paramref name="extensions" /> from the current verification.
    /// Any existing verified file for an excluded extension is treated as pending deletion.
    /// Intended for converters that emit a source document (eg <c>pdf</c> or <c>docx</c>) alongside
    /// the info file and rendered pages, where committing the source document is not wanted.
    /// </summary>
    public void ExcludeTargets(params string[] extensions) =>
        excludedTargets = VerifierSettings.AddExcludedTargets(excludedTargets, extensions);
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
