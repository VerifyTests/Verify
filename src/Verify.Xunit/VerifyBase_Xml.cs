namespace VerifyXunit;

public partial class VerifyBase
{
    public SettingsTask VerifyXml(
        string? target,
        VerifySettings? settings = null) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile);

    public SettingsTask VerifyXml(
        Task<string> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile);

    public SettingsTask VerifyXml(
        ValueTask<string> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile);

    public SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile);

    public SettingsTask VerifyXml(
        Task<Stream> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile);

    public SettingsTask VerifyXml(
        ValueTask<Stream> target,
        VerifySettings? settings = null) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile);
}