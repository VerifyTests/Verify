namespace VerifyXunit;

public partial class VerifyBase
{
    public SettingsTask Throws(
        Action target,
        VerifySettings? settings = null) =>
        Verifier.Throws(target, settings ?? this.settings, sourceFile);

    public SettingsTask Throws(
        Func<object?> target,
        VerifySettings? settings = null) =>
        Verifier.Throws(target, settings ?? this.settings, sourceFile);

    public SettingsTask Throws(
        Func<Task> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsTask(target, settings ?? this.settings, sourceFile);

    public SettingsTask Throws(
        Func<ValueTask> target,
        VerifySettings? settings = null) =>
        Verifier.ThrowsValueTask(target, settings ?? this.settings, sourceFile);
}