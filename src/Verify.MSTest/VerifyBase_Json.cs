namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        StringBuilder? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        Task<StringBuilder> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        ValueTask<StringBuilder> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyJson(target, settings, sourceFile);
}