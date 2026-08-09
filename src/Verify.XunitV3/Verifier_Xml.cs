namespace VerifyXunit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyXml(target));
}