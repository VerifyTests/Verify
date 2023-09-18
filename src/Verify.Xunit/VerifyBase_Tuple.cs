#if !NETSTANDARD2_0 && !NET462
namespace VerifyXunit
{
    public partial class VerifyBase
    {
        [Pure]
        public SettingsTask VerifyTuple(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null) =>
            Verifier.VerifyTuple(target, settings ?? this.settings, sourceFile);
    }
}
#endif