namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Verify(
        byte[] target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target));

    public SettingsTask Verify(
        Task<byte[]> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            async _ => await _.Verify(await target));

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// Differs from <code>Verify(FileInfo path)</code> which will verify the full path.
    /// </summary>
    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));
}