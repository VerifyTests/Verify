namespace VerifyXunit;

public partial class VerifyBase
{

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyDirectory(path,include, pattern, options, settings ?? this.settings, info: info, sourceFile: sourceFile);

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// Differs from passing <see cref="DirectoryInfo"/> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    public SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyDirectory(path.FullName, include, pattern, options, settings ?? this.settings, info: info, sourceFile: sourceFile);

#else

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyDirectory(path, include, pattern, option, settings ?? this.settings, sourceFile);

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// Differs from passing <see cref="DirectoryInfo"/> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    public SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyDirectory(path.FullName, include, pattern, option, settings ?? this.settings, sourceFile);

#endif
}