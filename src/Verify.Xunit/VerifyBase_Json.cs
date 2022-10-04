namespace VerifyXunit;

public partial class VerifyBase
{
    public SettingsTask VerifyJson(
        string? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyJson(target, settings, sourceFile);
    }

    public SettingsTask VerifyJson(
        Stream? target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyJson(target, settings, sourceFile);
    }
}