// ReSharper disable RedundantSuppressNullableWarningExpression
namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static Combination Combination(
        string name,
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool? header = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return new(
            captureExceptions,
            settings,
            header,
            sourceFile,
            lineNumber,
            (settings, sourceFile, line, verify) => Verify(settings, assembly, sourceFile, line, name, verify));
    }
}