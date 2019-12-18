#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null)
        {
            return GetVerifier().Verify(expression, settings);
        }
    }
}
#endif