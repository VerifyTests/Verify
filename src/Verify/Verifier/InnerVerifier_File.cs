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
            if (!settings.HasExtension())
            {
                settings = new VerifySettings(settings);
                settings.UseExtension(Extensions.GetExtension(path));
            }

            return Verify(FileHelpers.OpenRead(path), settings);
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