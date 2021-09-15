#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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