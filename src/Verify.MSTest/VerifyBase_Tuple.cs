#if !NETSTANDARD2_0
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public async Task Verify(
            Expression<Func<ITuple>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            using var verifier = BuildVerifier(settings, sourceFile);
            await verifier.Verify(target);
        }
    }
}
#endif