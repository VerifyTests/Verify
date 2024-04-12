namespace VerifyMSTest;

public partial class Verifier
{
    [Pure]
    public SettingsTask VerifyTuple(
        Expression<Func<ITuple>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyTuple(target));
}