namespace VerifyXunit;

public static partial class Verifier
{
#if NET6_0_OR_GREATER

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include, pattern, options, info, fileScrubber), true);

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// Differs from passing <see cref="DirectoryInfo" /> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        VerifyDirectory(path.FullName, include, pattern, options, settings, info, fileScrubber, sourceFile);

#else
    /// <summary>
    /// Verifies the contents of <paramref name="path"/>.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.VerifyDirectory(path, include, pattern, option, info, fileScrubber),
            true);

    /// <summary>
    /// Verifies the contents of <paramref name="path"/>.
    /// Differs from passing <see cref="DirectoryInfo"/> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        VerifyDirectory(path.FullName, include, pattern, option, settings, info, fileScrubber, sourceFile);

#endif
}