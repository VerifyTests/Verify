using VerifyTests;

namespace VerifyNUnit;

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

    public SettingsTask ThrowsTask(
        Func<Task> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.ThrowsTask(target, settings, sourceFile);
    }

    public SettingsTask ThrowsValueTask<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.ThrowsTask(target, settings, sourceFile);
    }

    public SettingsTask ThrowsValueTask(
        Func<ValueTask> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.ThrowsValueTask(target, settings, sourceFile);
    }

    public SettingsTask ThrowsValueTask<T>(
        Func<ValueTask<T>> target,
        VerifySettings? settings = null)
    {
        settings ??= this.settings;
        return Verifier.ThrowsValueTask(target, settings, sourceFile);
    }
}