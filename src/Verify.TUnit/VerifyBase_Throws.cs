namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Throws(
        Action target,
        VerifySettings? settings = null) =>
        Verifier.Throws(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null) =>
        Verifier.Throws(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsTask(
        Func<Task> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsValueTask<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsValueTask(
        Func<ValueTask> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask ThrowsValueTask<T>(
        Func<ValueTask<T>> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile);
}