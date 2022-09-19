namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask Verify(
        byte[] target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<byte[]> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyFile(path, settings, sourceFile);
    }

    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// Differs from <code>Verify(FileInfo path)</code> which will verify the full path.
    /// </summary>
    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyFile(path, settings, sourceFile);
    }
}