#if !NETSTANDARD2_0 && !NET461
namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        [Obsolete("Use VerifyTuple")]
        public SettingsTask Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return VerifyTuple(expression, settings, sourceFile);
        }

        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.VerifyTuple(target));
        }
    }
}
#endif