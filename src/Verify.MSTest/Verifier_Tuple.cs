#if !NET462
namespace VerifyMSTest;

partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyTuple(
        Expression<Func<ITuple>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyTuple(target));
}
#endif