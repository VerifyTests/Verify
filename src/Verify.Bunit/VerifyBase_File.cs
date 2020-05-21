using System.IO;
using System.Threading.Tasks;
using Verify;

namespace VerifyBunit
{
    public partial class VerifyBase
    {
        public async Task Verify(
            byte[] target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(target, settings);
            Flush();
        }

        public async Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(await target, settings);
            Flush();
        }

        public async Task VerifyFile(
            string path,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.VerifyFile(path, settings);
            Flush();
        }

        public async Task VerifyFile(
            FileInfo file,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.VerifyFile(file, settings);
            Flush();
        }
    }
}