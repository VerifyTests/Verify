using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static async Task Verify(
            byte[] target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.Verify(target, settings);
        }

        public static async Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            var bytes = await target;
            await verifier.Verify(bytes, settings);
        }

        public static async Task VerifyFile(
            string path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.VerifyFile(path, settings);
        }

        public static async Task VerifyFile(
            FileInfo path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.VerifyFile(path, settings);
        }
    }
}