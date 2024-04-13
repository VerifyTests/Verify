namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask Verify(
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, extension, settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, extension, settings, sourceFile);
}