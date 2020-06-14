using System.IO;
using System.Threading.Tasks;

namespace VerifyTesting
{
    public delegate Task<CompareResult> Compare(VerifySettings settings, Stream received, Stream verified);
}