using System.Threading.Tasks;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(string target)
        {
            return verifier.Verify(target);
        }
    }
}