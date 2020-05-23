using System.Collections.Generic;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.Verify(target, settings);
        }

        public Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.Verify(target, settings);
        }

        public Task Verify<T>(
            T target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.Verify(target, settings);
        }
    }
}