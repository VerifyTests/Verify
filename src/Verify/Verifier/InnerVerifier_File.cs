using System.IO;
using System.Threading.Tasks;
using EmptyFiles;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task VerifyFile(
            string path,
            VerifySettings settings)
        {
            Guard.FileExists(path, nameof(path));
            var extension = settings.extension ?? Extensions.GetExtension(path);

            return VerifyStream(settings, FileHelpers.OpenRead(path), extension);
        }

        public Task VerifyFile(
            FileInfo target,
            VerifySettings settings)
        {
            Guard.AgainstNull(target, nameof(target));
            return VerifyFile(target.FullName, settings);
        }
    }
}