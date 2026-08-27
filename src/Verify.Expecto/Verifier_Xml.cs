namespace VerifyExpecto;

// ReSharper disable RedundantSuppressNullableWarningExpression
public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyXml(
        string name,
        [StringSyntax(StringSyntaxAttribute.Xml)]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, lineNumber, name, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        string name,
        [StringSyntax(StringSyntaxAttribute.Xml)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, lineNumber, name, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        string name,
        [StringSyntax(StringSyntaxAttribute.Xml)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, lineNumber, name, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        string name,
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, lineNumber, name, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        string name,
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, lineNumber, name, _ => _.VerifyXml(target));

    [Pure]
    public static SettingsTask VerifyXml(
        string name,
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, lineNumber, name, _ => _.VerifyXml(target));
}