// ReSharper disable InconsistentNaming
namespace VerifyMSTest;

public static partial class Verifier
{
    [Pure]
    public static Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        new(
            captureExceptions,
            settings,
            sourceFile,
            (settings, sourceFile, verify) => Verify(settings, sourceFile, verify));
}