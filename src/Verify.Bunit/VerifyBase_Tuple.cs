#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyBunit
{
    public partial class VerifyBase
    {
        public async Task Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(expression, settings);
            Flush();
        }
    }
}
#endif