namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// </summary>
    [Pure]
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyFile(path, settings, info, sourceFile);

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// Differs from passing <see cref="FileInfo" /> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    [Pure]
    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyFile(path, settings, info, sourceFile);
}