using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static Task Verify(
            byte[] target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            var verifier = GetVerifier(sourceFile);
            return verifier.Verify(target, settings);
        }

        public static async Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            var verifier = GetVerifier(sourceFile);
            var bytes = await target;
            await verifier.Verify(bytes, settings);
        }

        public static Task VerifyFile(
            string path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            var verifier = GetVerifier(sourceFile);
            return verifier.VerifyFile(path, settings);
        }

        public static Task VerifyFile(
            FileInfo path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            var verifier = GetVerifier(sourceFile);
            return verifier.VerifyFile(path, settings);
        }
    }
}