using System.IO;
using System.Threading.Tasks;

namespace Verify
{
    public delegate Task<CompareResult> Compare(VerifySettings settings, Stream received, Stream verified);
}