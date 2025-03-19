namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool? header = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Combination(captureExceptions, settings ?? this.settings, header, sourceFile);
}