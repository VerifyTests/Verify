#if !NETSTANDARD2_0 && !NET461
namespace VerifyNUnit;

public static partial class Verifier
{
    public static SettingsTask VerifyTuple(
        Expression<Func<ITuple>> expression,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyTuple(expression));
    }
}
#endif