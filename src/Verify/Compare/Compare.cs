using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task<CompareResult> Compare(Stream received, Stream verified, FilePair filePair, IReadOnlyDictionary<string, object> context);
}