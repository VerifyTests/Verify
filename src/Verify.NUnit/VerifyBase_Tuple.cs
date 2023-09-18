#if !NETSTANDARD2_0 && !NET462
namespace VerifyNUnit
{
    public partial class VerifyBase
    {
        [Pure]
        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null) =>
            Verifier.VerifyTuple(target, settings ?? this.settings);
    }
}
#endif