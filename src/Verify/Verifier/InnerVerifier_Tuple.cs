#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public Task Verify(
        Expression<Func<ITuple>> expression,
        VerifySettings? settings = null)
    {
        var dictionary = TupleConverter.ExpressionToDictionary(expression);
        return Verify(dictionary, settings);
    }
}

#endif