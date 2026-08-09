namespace VerifyXunit;

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
    public SettingsTask Throws(
        Func<Task> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Throws(
        Func<ValueTask> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile, lineNumber);
}