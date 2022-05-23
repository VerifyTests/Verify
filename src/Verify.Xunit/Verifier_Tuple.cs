#if !NETSTANDARD2_0 && !NET461
namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static SettingsTask VerifyTuple(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "") =>
            Verify(settings, sourceFile, _ => _.VerifyTuple(expression));
    }
}
#endif