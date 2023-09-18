namespace VerifyXunit;

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
    public SettingsTask Throws(
        Func<Task> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask Throws(
        Func<ValueTask> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile);
}