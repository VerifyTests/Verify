namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyFile(path, settings, sourceFile);
    }

    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyFile(path, settings, sourceFile);
    }
}