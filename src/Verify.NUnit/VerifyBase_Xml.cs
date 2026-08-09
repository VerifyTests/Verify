namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        string? target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask VerifyXml(
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask VerifyXml(
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyXml(target, settings ?? this.settings, sourceFile, lineNumber);
}