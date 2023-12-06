// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// </summary>
    public static Task<VerifyResult> VerifyFile(
        string name,
        string path,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFile(path, info));
    }

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// Differs from passing <see cref="FileInfo" /> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    public static Task<VerifyResult> VerifyFile(
        string name,
        FileInfo path,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFile(path, info));
    }
}