#if !NETSTANDARD2_0 && !NET462
namespace VerifyNUnit
{
    public partial class VerifyBase
    {
        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyTuple(target, settings);
        }
    }
}
#endif