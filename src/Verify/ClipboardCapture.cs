using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new StringBuilder();
    static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public static Task AppendMove(string received, string verified)
    {
        var command = $"move /Y \"{received}\" \"{verified}\"";
        return Append(command);
    }

    public static Task AppendDelete(string verified)
    {
        var command = $"del \"{verified}\"";
        return Append(command);
    }

    static async Task Append(string command)
    {
        await semaphore.WaitAsync();

        try
        {
            builder.AppendLine(command);
            await Clipboard.SetTextAsync(builder.ToString());
        }
        finally
        {
            semaphore.Release();
        }
    }
}