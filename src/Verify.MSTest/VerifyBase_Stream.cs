using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyMSTest
{
    #region VerifyBinaryMSTest
    public partial class VerifyBase
    {
        public async Task VerifyBinary(
            Stream input,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile, settings);
            await verifier.VerifyBinary(input, settings);
        }

        public async Task VerifyBinary<T>(
            T input,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile, settings);
            await verifier.VerifyBinary(input, settings);
        }

        public async Task VerifyBinary(
            IEnumerable<Stream> streams,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile, settings);
            await verifier.VerifyBinary(streams, settings);
        }
    }
    #endregion
}