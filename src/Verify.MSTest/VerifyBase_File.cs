using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public async Task Verify(
            byte[] target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.Verify(target, settings);
        }

        public async Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.Verify(await target, settings);
        }

        public async Task VerifyFile(
            string path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.VerifyFile(path, settings);
        }

        public async Task VerifyFile(
            FileInfo path,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.VerifyFile(path, settings);
        }
    }
}