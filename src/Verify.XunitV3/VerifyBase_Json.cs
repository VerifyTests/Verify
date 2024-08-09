namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        string? target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        Task<string> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        [StringSyntax(StringSyntaxAttribute.Json)]
        ValueTask<string> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        Stream? target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        Task<Stream> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        ValueTask<Stream> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);
}