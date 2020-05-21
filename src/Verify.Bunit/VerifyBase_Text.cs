using System.Threading.Tasks;
using Verify;

namespace VerifyBunit
{
    public partial class VerifyBase
    {
        public async Task Verify(string target, VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(target, settings);
            Flush();
        }
    }
}