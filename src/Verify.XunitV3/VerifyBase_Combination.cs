namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        bool? header = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Combination(captureExceptions, settings ?? this.settings, header, sourceFile, lineNumber);
}