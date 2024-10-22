namespace VerifyExpecto;

// ReSharper disable RedundantSuppressNullableWarningExpression
public static partial class Verifier
{
    [Pure]
    public static Task<VerifyResult> VerifyXml(
        string name,
        [StringSyntax(StringSyntaxAttribute.Xml)]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    [Pure]
    public static Task<VerifyResult> VerifyXml(
        string name,
        [StringSyntax(StringSyntaxAttribute.Xml)]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    [Pure]
    public static Task<VerifyResult> VerifyXml(
        string name,
        [StringSyntax(StringSyntaxAttribute.Xml)]
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    [Pure]
    public static Task<VerifyResult> VerifyXml(
        string name,
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    [Pure]
    public static Task<VerifyResult> VerifyXml(
        string name,
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    [Pure]
    public static Task<VerifyResult> VerifyXml(
        string name,
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));
}