using System.IO;
using System.Text;
using System.Threading.Tasks;

static class FileHelpers
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
        return new(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
    }

    public static FileStream OpenRead(string path)
    {
        return new(path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);
    }

    static Task<StringBuilder> ReadText(FileStream stream)
    {
        return stream.ReadAsString();
    }

#if NETSTANDARD2_1 || NET5_0
    public static Task WriteText(string filePath, string text)
    {
        return File.WriteAllTextAsync(filePath, text);
    }

    public static async Task<StringBuilder> ReadText(string filePath)
    {
        await using var stream = OpenRead(filePath);
        return await ReadText(stream);
    }

    public static async Task WriteStream(string filePath, Stream stream)
    {
        await using var fileStream = OpenWrite(filePath);
        await stream.CopyToAsync(fileStream);
    }
#else
    public static async Task WriteText(string filePath, string text)
    {
        var encodedText = Utf8NoBOM.GetBytes(text);

        using var fileStream = OpenWrite(filePath);
        await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
    }

    public static async Task WriteStream(string filePath, Stream stream)
    {
        using var fileStream = OpenWrite(filePath);
        await stream.CopyToAsync(fileStream);
    }

    public static async Task<StringBuilder> ReadText(string filePath)
    {
        using var stream = OpenRead(filePath);
        return await ReadText(stream);
    }
#endif
}