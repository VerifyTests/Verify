using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify<T>(
            Task<T> task,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.Verify(task, settings);
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