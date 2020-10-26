using System.IO;
using System.Threading.Tasks;
using EmptyFiles;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task VerifyFile(string path)
        {
            Guard.FileExists(path, nameof(path));
            var extension = settings.extension ?? Extensions.GetExtension(path);
            return VerifyStream(settings, FileHelpers.OpenRead(path), extension);
        }

        public Task VerifyFile(FileInfo target)
        {
            Guard.AgainstNull(target, nameof(target));
            return VerifyFile(target.FullName);
        }
    }
}