#if !NETSTANDARD2_0 && !NET462
namespace VerifyNUnit;

public static partial class Verifier
{
    public static SettingsTask VerifyTuple(
        Expression<Func<ITuple>> expression,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyTuple(expression));
}
#endif