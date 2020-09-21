#if NETSTANDARD2_1 || NET5
using System.IO;
using System.Text;
using System.Threading.Tasks;

static partial class FileHelpers
{
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
}
#endif