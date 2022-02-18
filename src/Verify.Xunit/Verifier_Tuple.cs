#if !NETSTANDARD2_0 && !NET461
namespace VerifyXunit
{
    public static partial class Verifier
    {
        [Obsolete("Use VerifyTuple")]
        public static SettingsTask Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyTuple(expression, settings, sourceFile);
        }

        public static SettingsTask VerifyTuple(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.VerifyTuple(expression));
        }
    }
}
#endif