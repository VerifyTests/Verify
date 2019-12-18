using System.IO;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task VerifyFile(
            string path,
            VerifySettings? settings = null)
        {
            return GetVerifier().VerifyFile(path, settings);
        }

        public Task VerifyFile(
            FileInfo file,
            VerifySettings? settings = null)
        {
            return GetVerifier().VerifyFile(file, settings);
        }
    }
}