namespace VerifyMSTest;

[TestClass]
[UsesVerify]
public abstract partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, rawTargets, settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(targets, settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);
}