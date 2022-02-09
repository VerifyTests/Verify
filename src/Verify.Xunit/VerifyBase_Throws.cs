namespace VerifyXunit;

public partial class VerifyBase
{
    public SettingsTask Throws(
        Action target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Throws(target, settings, sourceFile);
    }

    public SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.Throws(target, settings, sourceFile);
    }

    public SettingsTask Throws(
        Func<Task> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.ThrowsTask(target, settings, sourceFile);
    }

    public SettingsTask Throws(
        Func<ValueTask> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.ThrowsValueTask(target, settings, sourceFile);
    }
}