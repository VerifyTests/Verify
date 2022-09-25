// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    public static Task<VerifyResult> Verify(
        string name,
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyString(target));
    }

    public static Task<VerifyResult> Verify(
        string name,
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyString(target));
    }

    public static Task<VerifyResult> Verify(
        string name,
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyString(target, extension));
    }

    public static Task<VerifyResult> Verify(
        string name,
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyString(target, extension));
    }
}