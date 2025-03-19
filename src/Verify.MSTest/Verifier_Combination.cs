namespace VerifyMSTest;

public static partial class Verifier
{
    [Pure]
    public static Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool header = false,
        [CallerFilePath] string sourceFile = "") =>
        new(
            captureExceptions,
            settings,
            header,
            sourceFile,
            (settings, sourceFile, verify) => Verify(settings, sourceFile, verify));
}