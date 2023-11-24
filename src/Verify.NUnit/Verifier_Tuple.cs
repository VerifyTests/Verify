namespace VerifyNUnit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyTuple(
        Expression<Func<ITuple>> expression,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyTuple(expression));
}