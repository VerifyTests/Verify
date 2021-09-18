#if !NETSTANDARD2_0
using System.Linq.Expressions;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task VerifyTuple(Expression<Func<ITuple>> target)
        {
            var dictionary = TupleConverter.ExpressionToDictionary(target);
            return Verify(dictionary);
        }
    }
}

#endif