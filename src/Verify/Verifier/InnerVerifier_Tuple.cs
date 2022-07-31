﻿#if !NETSTANDARD2_0 && !NET461
partial class InnerVerifier
{
    public Task<VerifyResult> VerifyTuple(Expression<Func<ITuple>> target)
    {
        var dictionary = TupleConverter.ExpressionToDictionary(target);
        return Verify(dictionary);
    }
}
#endif