namespace VerifyMSTest;

partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyTuple(
        Expression<Func<ITuple>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyTuple(target));
}