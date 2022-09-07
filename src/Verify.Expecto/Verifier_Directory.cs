// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    public static Task<VerifyResult> VerifyDirectory(
        string name,
        string path,
        Func<string, bool> include,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyDirectory(path, include), true);
    }

    public static Task<VerifyResult> VerifyDirectory(
        string name,
        DirectoryInfo path,
        Func<string, bool> include,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyDirectory(path, include), true);
    }
}