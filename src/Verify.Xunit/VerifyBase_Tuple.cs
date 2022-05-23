#if !NETSTANDARD2_0 && !NET461
namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyTuple(target, settings, sourceFile);
        }
    }
}
#endif