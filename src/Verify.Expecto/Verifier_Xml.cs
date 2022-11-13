namespace VerifyExpecto;

// ReSharper disable RedundantSuppressNullableWarningExpression

public static partial class Verifier
{
    public static Task<VerifyResult> VerifyXml(
        string name,
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    public static Task<VerifyResult> VerifyXml(
        string name,
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    public static Task<VerifyResult> VerifyXml(
        string name,
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    public static Task<VerifyResult> VerifyXml(
        string name,
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    public static Task<VerifyResult> VerifyXml(
        string name,
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));

    public static Task<VerifyResult> VerifyXml(
        string name,
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyXml(target));
}