using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new StringBuilder();
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);

    public static async Task Append(string received, string verified)
    {
        var command = $"cmd /c move /Y \"{received}\" \"{verified}\"";
        await semaphoreSlim.WaitAsync();

        try
        {
            builder.AppendLine(command);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        await Clipboard.SetText(builder.ToString());
    }
}