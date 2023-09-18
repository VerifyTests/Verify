namespace VerifyMSTest;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Verify(
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target));

    [Pure]
    public SettingsTask Verify(
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target));

    [Pure]
    public SettingsTask Verify(
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target, extension));

    [Pure]
    public SettingsTask Verify(
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target, extension));
}