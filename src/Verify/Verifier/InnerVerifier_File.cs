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
            string extension;
            if (!settings.HasExtension())
            {
                extension = Extensions.GetExtension(path);
                settings = new VerifySettings(settings);
                settings.UseExtension(extension);
            }
            else
            {
                extension = settings.extension!;
            }

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