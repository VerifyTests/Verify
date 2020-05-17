using System.IO;
using System.Text;
using System.Threading.Tasks;

static partial class FileHelpers
{
    public static readonly Encoding Utf8NoBOM = new UTF8Encoding(false, true);

    public static void DeleteIfEmpty(string path)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists && fileInfo.Length == 0)
        {
            fileInfo.Delete();
        }
    }

    static FileStream OpenWrite(string filePath)
    {
        return new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
    }

    public static FileStream OpenRead(string path)
    {
        return new FileStream(path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);
    }

    static async Task<StringBuilder> ReadText(FileStream stream)
    {
        var builder = new StringBuilder();

        var buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            var text = Utf8NoBOM.GetString(buffer, 0, numRead);
            builder.Append(text);
        }

        return builder;
    }
}