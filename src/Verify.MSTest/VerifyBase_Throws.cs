namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask Throws(
        Action target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Throws(target, settings, sourceFile);

    [Pure]
    public SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Throws(target, settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsTask(
        Func<Task> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.ThrowsTask(target, settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsTask<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.ThrowsTask(target, settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsValueTask(
        Func<ValueTask> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.ThrowsValueTask(target, settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsValueTask<T>(
        Func<ValueTask<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.ThrowsValueTask(target, settings, sourceFile);
}