using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task<CompareResult> StreamCompare(Stream received, Stream verified, IReadOnlyDictionary<string, object> context);
}