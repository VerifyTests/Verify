namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Throws(
        Action target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Throws(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Throws(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask ThrowsTask(
        Func<Task> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask ThrowsValueTask<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask ThrowsValueTask(
        Func<ValueTask> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask ThrowsValueTask<T>(
        Func<ValueTask<T>> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile, lineNumber);
}