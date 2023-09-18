namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyJson(
        string? target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
        Task<string> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyJson(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyJson(
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