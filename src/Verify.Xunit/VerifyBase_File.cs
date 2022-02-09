namespace VerifyXunit;

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