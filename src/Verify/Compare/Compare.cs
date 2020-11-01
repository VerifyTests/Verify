using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task<CompareResult> Compare(VerifySettings settings, Stream received, Stream verified, FilePair filePair);
}