using System.IO;
using System.Linq;
using System.Threading.Tasks;

static partial class FileHelpers
{
    public static void DeleteIfEmpty(string path)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists && fileInfo.Length == 0)
        {
            fileInfo.Delete();
        }
    }

    public static async Task WriteStream(string filePath, Stream stream)
    {
        using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
        await stream.CopyToAsync(fileStream);
    }

    public static bool FilesEqual(string path1, string path2)
    {
        if (new FileInfo(path1).Length != new FileInfo(path2).Length)
        {
            return false;
        }
        return File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2));
    }

    public static void WriteEmpty(string path)
    {
        File.Create(path).Dispose();
    }
}