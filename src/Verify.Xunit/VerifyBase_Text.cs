using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(string target, VerifySettings? settings = null)
        {
            return verifier.Verify(target, settings);
        }
    }
}