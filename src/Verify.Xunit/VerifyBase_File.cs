namespace VerifyXunit;

public partial class VerifyBase
{
    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyFile(path, settings ?? this.settings, info, sourceFile);

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// Differs from passing <see cref="FileInfo"/> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyFile(path, settings ?? this.settings, info, sourceFile);
}