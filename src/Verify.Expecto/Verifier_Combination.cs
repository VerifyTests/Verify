// ReSharper disable RedundantSuppressNullableWarningExpression
namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static Combination Combination(
        string name,
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool header = false,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return new(
            captureExceptions,
            settings,
            header,
            sourceFile,
            (settings, sourceFile, verify) => Verify(settings, assembly, sourceFile, name, verify));
    }
}