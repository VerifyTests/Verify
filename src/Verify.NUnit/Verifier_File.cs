namespace VerifyNUnit;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// Differs from passing <see cref="FileInfo"/> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    public static SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));
}