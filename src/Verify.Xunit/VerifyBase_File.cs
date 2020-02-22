using System.IO;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(
            byte[] target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.Verify(target, settings);
        }

        public async Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            await verifier.Verify(await target, settings);
        }

        public Task VerifyFile(
            string path,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.VerifyFile(path, settings);
        }

        public Task VerifyFile(
            FileInfo file,
            VerifySettings? settings = null)
        {
            var verifier = GetVerifier();
            return verifier.VerifyFile(file, settings);
        }
    }
}