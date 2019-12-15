using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyNUnit
{
    #region VerifyBinaryNUnit
    public static partial class Verifier
    {
        public static async Task VerifyBinary(
            Stream input,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.VerifyBinary(input, settings);
        }

        public static async Task VerifyBinary(
            IEnumerable<Stream> streams,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.VerifyBinary(streams, settings);
        }
    }
    #endregion
}