namespace VerifyXunit;

public partial class VerifyBase
{
    public SettingsTask VerifyDirectory(
        string path,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyFile(path, settings, sourceFile);
    }

    public SettingsTask VerifyDirectory(
        DirectoryInfo path,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(path, settings, sourceFile);
    }
}