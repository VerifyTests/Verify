using System.IO;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(
            byte[] target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Verify(target, settings, sourceFile);
        }

        public Task Verify(
            Task<byte[]> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Verify(target, settings, sourceFile);
        }

        public Task VerifyFile(
            string path,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyFile(path, settings, sourceFile);
        }

        public Task VerifyFile(
            FileInfo path,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.VerifyFile(path, settings, sourceFile);
        }
    }
}