using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new StringBuilder();
    static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    static string moveCommand;
    static string deleteCommand;
    
    static ClipboardCapture()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            moveCommand = "move /Y \"{0}\" \"{1}\""; 
            deleteCommand = "del \"{0}\"";
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            moveCommand = "mv -f \"{0}\" \"{1}\"";
            deleteCommand = "rm -f \"{0}\"";
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            moveCommand = "mv -f \"{0}\" \"{1}\"";
            deleteCommand = "rm -f \"{0}\"";
            return;
        }

        throw new Exception($"OS not supported: {RuntimeInformation.OSDescription}");
    }
    
    public static Task AppendMove(string received, string verified)
    {
        return Append(string.Format(moveCommand, received, verified));
    }

    public static Task AppendDelete(string verified)
    {
        return Append(string.Format(deleteCommand, verified));
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