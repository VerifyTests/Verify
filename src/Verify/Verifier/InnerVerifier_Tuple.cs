#if !NETSTANDARD2_0 && !NET462
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyTuple(Expression<Func<ITuple>> target)
    {
        var dictionary = TupleConverter.ExpressionToDictionary(target);
        return Verify(dictionary);
    }
}
#endif