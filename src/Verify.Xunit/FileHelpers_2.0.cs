#if(!NETSTANDARD2_1)
using System.IO;
using System.Threading.Tasks;
using System.Text;

static partial class FileHelpers
{
    public static async Task WriteText(string filePath, string text)
    {
        var encodedText = Encoding.UTF8.GetBytes(text);

        using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
        await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
    }

    public static async Task<string> ReadText(string filePath)
    {
        using var sourceStream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);
        var builder = new StringBuilder();

        var buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            var text = Encoding.UTF8.GetString(buffer, 0, numRead);
            builder.Append(text);
        }

        return builder.ToString();
    }
}
#endif