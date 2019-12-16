using System.IO;
using System.Threading.Tasks;
using Verify;

partial class Verifier
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
            settings.UseExtension(Path.GetExtension(path).Substring(1));
        }

        return Verify(File.OpenRead(path), settings);
    }

    public Task VerifyFile(
        FileInfo file,
        VerifySettings? settings = null)
    {
        Guard.AgainstNull(file, nameof(file));
        return VerifyFile(file.FullName, settings);
    }
}