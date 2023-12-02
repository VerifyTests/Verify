namespace VerifyFixie;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Throws(
        Action target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Throws(target));

    [Pure]
    public static SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Throws(target));

    [Pure]
    public static SettingsTask ThrowsTask(
        Func<Task> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.ThrowsTask(target));

    [Pure]
    public static SettingsTask ThrowsTask<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.ThrowsTask(target));

    [Pure]
    public static SettingsTask ThrowsValueTask(
        Func<ValueTask> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.ThrowsValueTask(target));

    [Pure]
    public static SettingsTask ThrowsValueTask<T>(
        Func<ValueTask<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.ThrowsValueTask(target));
}