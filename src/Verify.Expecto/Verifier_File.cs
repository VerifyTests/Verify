// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    public static Task<VerifyResult> Verify(
        string name,
        byte[] target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task<VerifyResult> Verify(
        string name,
        Task<byte[]> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(
            settings,
            assembly,
            sourceFile,
            name,
            async _ =>
            {
                var bytes = await target;
                return await _.Verify(bytes);
            });
    }

    public static Task<VerifyResult> VerifyFile(
        string name,
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFile(path));
    }

    public static Task<VerifyResult> VerifyFile(
        string name,
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFile(path));
    }
}