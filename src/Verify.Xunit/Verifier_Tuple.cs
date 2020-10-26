#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static async Task Verify(
            Expression<Func<ITuple>> expression,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            using var verifier = GetVerifier(settings, sourceFile);
            await verifier.Verify(expression, settings);
        }
    }
}
#endif