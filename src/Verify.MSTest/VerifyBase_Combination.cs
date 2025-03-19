namespace VerifyMSTest;

public partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool header = false,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Combination(captureExceptions, settings ?? settings, sourceFile);
}