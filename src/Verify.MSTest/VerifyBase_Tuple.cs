#if !NETSTANDARD2_0 && !NET461
namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "") =>
            Verify(settings, sourceFile, _ => _.VerifyTuple(target));
    }
}
#endif