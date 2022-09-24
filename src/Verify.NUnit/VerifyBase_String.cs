namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask Verify(
        string target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<string> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify(
        string target,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, extension, settings, sourceFile);
    }

    public SettingsTask Verify(
        Task<string> target,
        string extension,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, extension, settings, sourceFile);
    }
}