namespace VerifyXunit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Verify(
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target));

    [Pure]
    public static SettingsTask Verify(
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target));

    [Pure]
    public static SettingsTask Verify(
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target, extension));

    [Pure]
    public static SettingsTask Verify(
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target, extension));
}