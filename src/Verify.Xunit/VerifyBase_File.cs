namespace VerifyXunit;

public partial class VerifyBase
{
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