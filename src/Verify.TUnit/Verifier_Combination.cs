namespace VerifyTUnit;

public static partial class Verifier
{
    [Pure]
    public static Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool? header = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        new(
            captureExceptions,
            settings,
            header,
            sourceFile,
            lineNumber,
            (settings, sourceFile, line, verify) => Verify(settings, sourceFile, line, verify));
}