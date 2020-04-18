#if(!NETSTANDARD2_1)
using System.IO;
using System.Threading.Tasks;
using System.Text;

static partial class FileHelpers
{
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

    public static async Task<string> ReadText(string filePath)
    {
        using var stream = OpenRead(filePath);
        var builder = new StringBuilder();

        var buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            var text = Utf8NoBOM.GetString(buffer, 0, numRead);
            builder.Append(text);
        }

        return builder.ToString();
    }
}
#endif