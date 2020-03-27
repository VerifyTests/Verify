using System.IO;
using System.Threading.Tasks;
using EmptyFiles;
using Verify;

partial class InnerVerifier
{
    public Task VerifyFile(
        string path,
        VerifySettings? settings = null)
    {
        Guard.FileExists(path, nameof(path));
        settings = settings.OrDefault();
        if (!settings.HasExtension())
        {
            settings = new VerifySettings(settings);
            settings.UseExtension(Extensions.GetExtension(path));
        }

        return Verify(FileHelpers.OpenRead(path), settings);
    }

    public Task VerifyFile(
        FileInfo file,
        VerifySettings? settings = null)
    {
        Guard.AgainstNull(file, nameof(file));
        return VerifyFile(file.FullName, settings);
    }
}