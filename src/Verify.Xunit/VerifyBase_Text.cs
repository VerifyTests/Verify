using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(string target, VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.Verify(target, settings);
        }
    }
}