#if !NET462
namespace VerifyNUnit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyTuple(
        Expression<Func<ITuple>> expression,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.VerifyTuple(expression));
}
#endif