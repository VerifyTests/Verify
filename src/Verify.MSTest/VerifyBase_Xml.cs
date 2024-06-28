namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyXml(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyXml(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyXml(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyXml(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyXml(
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyXml(target, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyXml(
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyXml(target, settings, sourceFile);
}