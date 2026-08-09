namespace VerifyMSTest;

public partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool? header = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Combination(captureExceptions, settings ?? settings, header, sourceFile, lineNumber);
}