using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static async Task VerifyFile(
            string path,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.VerifyFile(path);
        }

        public static async Task VerifyFile(
            FileInfo file,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.VerifyFile(file);
        }
    }
}