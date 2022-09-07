// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    public static Task<VerifyResult> VerifyDirectory(
        string name,
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyDirectory(path, include, pattern, options), true);
    }

    public static Task<VerifyResult> VerifyDirectory(
        string name,
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        VerifyDirectory(name, path.FullName, include, pattern, options, settings, sourceFile);

#else

    public static Task<VerifyResult> VerifyDirectory(
        string name,
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyDirectory(path, include, pattern, option), true);
    }

    public static Task<VerifyResult> VerifyDirectory(
        string name,
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        VerifyDirectory(name, path.FullName, include, pattern, option, settings, sourceFile);

#endif

}