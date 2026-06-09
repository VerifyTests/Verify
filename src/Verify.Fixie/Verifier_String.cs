namespace VerifyFixie;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target));

    [Pure]
    public static SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target));

    [Pure]
    public static SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target, extension));

    [Pure]
    public static SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyString(target, extension));
}