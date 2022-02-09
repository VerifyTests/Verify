namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask Verify<T>(
        T target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Verify(target, settings, sourceFile);
    }

    public SettingsTask VerifyJson(
        string target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyJson(target, settings, sourceFile);
    }

    public SettingsTask VerifyJson(
        JToken target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyJson(target, settings, sourceFile);
    }

    public SettingsTask VerifyJson(
        Stream target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.VerifyJson(target, settings, sourceFile);
    }
}