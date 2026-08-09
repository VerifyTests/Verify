namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyTuple(
        Expression<Func<ITuple>> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyTuple(target, settings ?? this.settings, sourceFile, lineNumber);
}