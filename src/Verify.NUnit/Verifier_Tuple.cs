#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static async Task Verify(
            Expression<Func<ITuple>> expression,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Verify(expression);
        }
    }
}
#endif