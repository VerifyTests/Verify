namespace VerifyTUnit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));

    [Pure]
    public static SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));

    [Pure]
    public static SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));

    [Pure]
    public static SettingsTask VerifyJson(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));

    [Pure]
    public static SettingsTask VerifyJson(
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));

    [Pure]
    public static SettingsTask VerifyJson(
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));
}