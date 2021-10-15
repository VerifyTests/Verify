using VerifyTests;

namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Throws(
        Action target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Throws(target));
    }

    public SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Throws(target));
    }

    public SettingsTask ThrowsTask(
        Func<Task> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.ThrowsTask(target));
    }

    public SettingsTask ThrowsTask<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.ThrowsTask(target));
    }

    public SettingsTask ThrowsValueTask(
        Func<ValueTask> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.ThrowsValueTask(target));
    }

    public SettingsTask ThrowsValueTask<T>(
        Func<ValueTask<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.ThrowsValueTask(target));
    }
}