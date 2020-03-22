using System.Threading.Tasks;
using Verify;

namespace VerifyBunit
{
    public partial class VerifyBase
    {
        public Task Verify(string target, VerifySettings? settings = null)
        {
            return GetVerifier().Verify(target, settings);
        }
    }
}