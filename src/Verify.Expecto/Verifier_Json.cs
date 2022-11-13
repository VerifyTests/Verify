namespace VerifyExpecto;

// ReSharper disable RedundantSuppressNullableWarningExpression

public static partial class Verifier
{
    public static Task<VerifyResult> VerifyJson(
        string name,
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyJson(target));

    public static Task<VerifyResult> VerifyJson(
        string name,
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyJson(target));

    public static Task<VerifyResult> VerifyJson(
        string name,
        ValueTask<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyJson(target));

    public static Task<VerifyResult> VerifyJson(
        string name,
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyJson(target));

    public static Task<VerifyResult> VerifyJson(
        string name,
        Task<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyJson(target));

    public static Task<VerifyResult> VerifyJson(
        string name,
        ValueTask<Stream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, Assembly.GetCallingAssembly()!, sourceFile, name, _ => _.VerifyJson(target));
}