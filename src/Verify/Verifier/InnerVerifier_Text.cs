using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        Task VerifyString(string target)
        {
            return VerifyBinary(Enumerable.Empty<ConversionStream>(),target, null);
        }
    }
}