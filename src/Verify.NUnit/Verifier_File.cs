using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static async Task Verify(
            byte[] target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Verify(target, settings);
        }

        public static async Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Verify(await target, settings);
        }

        public static async Task VerifyFile(
            string path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.VerifyFile(path, settings);
        }

        public static async Task VerifyFile(
            FileInfo path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.VerifyFile(path, settings);
        }
    }
}