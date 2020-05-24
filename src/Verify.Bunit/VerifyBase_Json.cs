using System.Collections.Generic;
using System.Threading.Tasks;
using Verify;

namespace VerifyBunit
{
    public partial class VerifyBase
    {
        public async Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(target, settings);
            Flush();
        }

        public async Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(target, settings);
            Flush();
        }

        public async Task Verify<T>(
            T target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(target, settings);
            Flush();
        }
    }
}