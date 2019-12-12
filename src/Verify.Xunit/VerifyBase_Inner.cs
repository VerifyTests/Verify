using System.Threading.Tasks;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(string input, string extension)
        {
            return verifier.Verify(input, extension);
        }
    }
}